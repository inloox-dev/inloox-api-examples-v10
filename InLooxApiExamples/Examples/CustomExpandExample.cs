using System;
using System.Linq;
using System.Threading.Tasks;
using Default;
using InLoox.ODataClient;
using InLoox.ODataClient.Extensions;

namespace InLooxApiExamples.Examples
{
    public class CustomExpandExample
    {
        private readonly Container _ctx;

        public CustomExpandExample(Container ctx)
        {
            _ctx = ctx;
        }

        public async Task PatchTaskCustomExpand()
        {
            var ceDefaults = (await _ctx.customexpanddefaultextend.ExecuteAsync())
                .ToList();
            var cedStatus = ceDefaults
                .FirstOrDefault(k => k.DisplayName == "Status");

            if (cedStatus == null)
            {
                Console.WriteLine("Custom expand 'Status' not found. Please create a custom expand type string on task");
                return;
            }

            var query = _ctx.workpackageview.OrderByDescending(k => k.CreatedDate);
            var task = (await ODataBasics.GetDSCollection(query)).FirstOrDefault();

            if (task == null)
            {
                Console.WriteLine("no task found. please create one");
                return;
            }
            
            Console.WriteLine($"Updating task {task.Name}");

            var customExpands = await _ctx.customexpandextend.GetCustomExpand(task.PlanningReservationId);
            var customExpand = customExpands
                .FirstOrDefault(k => k.CustomExpandDefaultId == cedStatus.CustomExpandDefaultId);

            if (customExpand == null)
            {
                Console.WriteLine("Custom expand not found");
                return;
            }

            await _ctx.customexpandextend.PatchCustomExpand(customExpand.CustomExpandId,
                "StringValue", "test");
        }
    }
}