using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Threading.Tasks;
using System.Web.Http;
using System.Web.Http.Description;
using Microsoft.Bot.Connector;
using Microsoft.ProjectOxford.Vision;
using Newtonsoft.Json;

namespace botcampdemo
{
    [BotAuthentication]
    public class MessagesController : ApiController
    {
        /// <summary>
        /// POST: api/Messages
        /// Receive a message from a user and reply to it
        /// </summary>
        public async Task<HttpResponseMessage> Post([FromBody]Activity activity)
        {
            Trace.TraceInformation(JsonConvert.SerializeObject(activity, Formatting.Indented));

            if (activity.Type == ActivityTypes.Message)
            {
                ConnectorClient connector = new ConnectorClient(new Uri(activity.ServiceUrl));
                // calculate something for us to return
                //int length = (activity.Text ?? string.Empty).Length;

                var fbData = JsonConvert.DeserializeObject<Class1>(activity.ChannelData.ToString());
                string url;
                try
                {
                    url = fbData.message.attachments.First()?.payload.url;
                    VisionServiceClient client = new VisionServiceClient("af0bc13ca86b401285d1759261666fc5");
                    var result = await client.AnalyzeImageAsync(url, new VisualFeature[]
                    {
                        VisualFeature.Description
                    });

                    url = $"{result.Description.Captions.First()?.Text} , Confidence:{result.Description.Captions.First()?.Confidence}";

                }
                catch (Exception)
                {
                    url = $"echo:{activity.Text}";
                }


                
                Activity reply = activity.CreateReply($"{url}");
                await connector.Conversations.ReplyToActivityAsync(reply);
            }
            else
            {
                HandleSystemMessage(activity);
            }
            var response = Request.CreateResponse(HttpStatusCode.OK);
            return response;
        }

        private Activity HandleSystemMessage(Activity message)
        {
            if (message.Type == ActivityTypes.DeleteUserData)
            {
                // Implement user deletion here
                // If we handle user deletion, return a real message
            }
            else if (message.Type == ActivityTypes.ConversationUpdate)
            {
                // Handle conversation state changes, like members being added and removed
                // Use Activity.MembersAdded and Activity.MembersRemoved and Activity.Action for info
                // Not available in all channels
            }
            else if (message.Type == ActivityTypes.ContactRelationUpdate)
            {
                // Handle add/remove from contact lists
                // Activity.From + Activity.Action represent what happened
            }
            else if (message.Type == ActivityTypes.Typing)
            {
                // Handle knowing tha the user is typing
            }
            else if (message.Type == ActivityTypes.Ping)
            {
            }

            return null;
        }
    }
}