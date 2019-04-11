using Default;
using InLooxOData;
using IQmedialab.InLoox.Data.BusinessObjects;
using Microsoft.OData.Client;
using System;
using System.Linq;

namespace InLooxnowClient.Examples
{
    public class TaskQueries
    {
        private readonly Container _ctx;

        public TaskQueries(Container ctx)
        {
            _ctx = ctx;
        }

        public WorkPackageView CreateInLooxTask()
        {
            var dsWk = ODataBasics.GetDSCollection<WorkPackageView>(_ctx);

            // first add then change properties
            var wk = new WorkPackageView();
            dsWk.Add(wk);
            wk.Name = "Pl" + DateTime.Now;

            _ctx.SaveChanges(SaveChangesOptions.PostOnlySetProperties);

            var query = _ctx.planningreservationdescription.Where(k =>
                k.PrimaryKey == wk.PlanningReservationId);
            var dsDescriptions = ODataBasics.GetDSCollection(query);
            var desc = dsDescriptions.First();
            desc.DescriptionHtml = "<b>wichtig</b>";

            _ctx.SaveChanges(SaveChangesOptions.PostOnlySetProperties);
            return wk;
        }

        public void UpdateInLooxTask(WorkPackageView task)
        {
            var query = _ctx.workpackageview
                .Where(k => k.PlanningReservationId == task.PlanningReservationId);
            var wks = ODataBasics.GetDSCollection(query);

            var wk = wks.First();
            wk.Name += " Geändert";

            _ctx.SaveChanges(SaveChangesOptions.PostOnlySetProperties);
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
