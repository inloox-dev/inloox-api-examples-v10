using Default;
using InLoox.ODataClient;
using InLoox.ODataClient.Data.BusinessObjects;
using Microsoft.OData.Client;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace InLooxnowClient.Examples
{
    public class TaskQueries
    {
        private readonly Container _ctx;

        public TaskQueries(Container ctx)
        {
            _ctx = ctx;
        }

        public async Task<WorkPackageView> CreateInLooxTask()
        {
            var dsWk = ODataBasics.GetDSCollection<WorkPackageView>(_ctx);

            // first add then change properties
            var wk = new WorkPackageView();
            dsWk.Add(wk);
            wk.Name = "Pl" + DateTime.Now;

            await _ctx.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties);

            var query = _ctx.planningreservationdescription.Where(k =>
                k.PrimaryKey == wk.PlanningReservationId);
            var dsDescriptions = await ODataBasics.GetDSCollection(query);
            var desc = dsDescriptions.First();
            desc.DescriptionHtml = "<b>wichtig</b>";

            await _ctx.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties);
            return wk;
        }

        public async Task UpdateInLooxTask(WorkPackageView task)
        {
            var query = _ctx.workpackageview
                .Where(k => k.PlanningReservationId == task.PlanningReservationId);
            var wks = await ODataBasics.GetDSCollection(query);

            var wk = wks.First();
            wk.Name += " Geändert";

            await _ctx.SaveChangesAsync(SaveChangesOptions.PostOnlySetProperties);
        }

        public void QueryInLooxTask()
        {
            var wks = _ctx.workpackageview
                .Where(k => k.ChangedDate > DateTime.Now.AddDays(-1))
                .ToList();

            Console.WriteLine("Tasks changed last 24 hours:");

            foreach (var wk in wks)
            {
                Console.WriteLine($"Task: {wk.Name} {wk.CreatedDate}");
                var desc = _ctx.planningreservationdescription.Where(k =>
                        k.PrimaryKey == wk.PlanningReservationId).ToList()
                    .FirstOrDefault();

                Console.WriteLine($"Description (HTML): {desc?.DescriptionHtml}");
            }
        }
    }
}
