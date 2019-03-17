/////////////////////////////////////////////////////////////////////
// Copyright (c) Autodesk, Inc. All rights reserved
// Written by Forge Partner Development
//
// Permission to use, copy, modify, and distribute this software in
// object code form for any purpose and without fee is hereby granted,
// provided that the above copyright notice appears in all copies and
// that both that copyright notice and the limited warranty and
// restricted rights notice below appear in all supporting
// documentation.
//
// AUTODESK PROVIDES THIS PROGRAM "AS IS" AND WITH ALL FAULTS.
// AUTODESK SPECIFICALLY DISCLAIMS ANY IMPLIED WARRANTY OF
// MERCHANTABILITY OR FITNESS FOR A PARTICULAR USE.  AUTODESK, INC.
// DOES NOT WARRANT THAT THE OPERATION OF THE PROGRAM WILL BE
// UNINTERRUPTED OR ERROR FREE.
/////////////////////////////////////////////////////////////////////
///
using System;
using Autodesk.Forge;
using Autodesk.Forge.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;

// For more information on enabling MVC for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace forgesample.Controllers
{
    [ApiController]
    public class ModelDerivativeController : ControllerBase
    {
        private IHubContext<ForgeCommunicationHub> _hubContext;
        public ModelDerivativeController(IHubContext<ForgeCommunicationHub> hubContext)
        {
            _hubContext = hubContext;
        }

        /// <summary>
        /// Start the translation job for a give bucketKey/objectName
        /// </summary>
        /// <param name="objModel"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("api/forge/modelderivative/jobs")]
        public async Task<dynamic> TranslateObject([FromBody]TranslateObjectModel objModel)
        {
            dynamic oauth = await OAuthController.GetInternalAsync();

            // prepare the webhook callback
            DerivativeWebhooksApi webhook = new DerivativeWebhooksApi();
            webhook.Configuration.AccessToken = oauth.access_token;
            dynamic existingHooks = await webhook.GetHooksAsync(DerivativeWebhookEvent.ExtractionFinished);

            // get the callback from your settings (e.g. web.config)
            string callbackUlr = OAuthController.GetAppSetting("FORGE_WEBHOOK_URL") + "/api/forge/callback/modelderivative";

            bool createHook = true; // need to create, we don't know if our hook is already there...
            foreach (KeyValuePair<string, dynamic> hook in new DynamicDictionaryItems(existingHooks.data))
            {
                if (hook.Value.scope.workflow.Equals(objModel.connectionId))
                {
                    // ok, found one hook with the same workflow, no need to create...
                    createHook = false;
                    if (!hook.Value.callbackUrl.Equals(callbackUlr))
                    {
                        await webhook.DeleteHookAsync(DerivativeWebhookEvent.ExtractionFinished, new System.Guid(hook.Value.hookId));
                        createHook = true; // ops, the callback URL is outdated, so delete and prepare to create again
                    }
                }
            }

            // need to (re)create the hook?
            if (createHook) await webhook.CreateHookAsync(DerivativeWebhookEvent.ExtractionFinished, callbackUlr, objModel.connectionId);

            // prepare the payload
            List<JobPayloadItem> outputs = new List<JobPayloadItem>()
            {
            new JobPayloadItem(
              JobPayloadItem.TypeEnum.Svf,
              new List<JobPayloadItem.ViewsEnum>()
              {
                JobPayloadItem.ViewsEnum._2d,
                JobPayloadItem.ViewsEnum._3d
              })
            };
            JobPayload job = new JobPayload(new JobPayloadInput(objModel.objectName), new JobPayloadOutput(outputs), new JobPayloadMisc(objModel.connectionId));


            // start the translation
            DerivativesApi derivative = new DerivativesApi();
            derivative.Configuration.AccessToken = oauth.access_token;
            dynamic jobPosted = await derivative.TranslateAsync(job, true/* force re-translate if already here, required data:write*/);
            return jobPosted;
        }


        [HttpPost]
        [Route("api/forge/callback/modelderivative")]
        public async Task<IActionResult> DerivativeCallback([FromBody]JObject body)
        {
            String connectionId = body["hook"]["scope"]["workflow"].Value<String>();
            await _hubContext.Clients.Client(connectionId).SendAsync("extractionFinished", body);

            return Ok();
        }

        
        /// <summary>
        /// Model for TranslateObject method
        /// </summary>
        public class TranslateObjectModel
        {
            public string bucketKey { get; set; }
            public string objectName { get; set; }
            public string connectionId { get; set; }
        }

    }
}
