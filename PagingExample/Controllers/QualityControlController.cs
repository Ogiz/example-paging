using System.Linq;
using System.Web.Mvc;
using DevExpress.Data.Filtering;
using DevExpress.Web.Mvc;
using PagingExample.Models;
using static PagingExample.Models.Constants;

namespace PagingExample.Controllers
{
    /// <summary>
    /// This controller is 
    /// </summary>
    public class QualityControlController : Controller
    {
        public ActionResult Index()
        {
            var model = GridViewExtension.GetViewModel(MEASUREMENTS_GRID_VIEW);
            if (model != null) return CustomBinding(model);

            model = new GridViewModel
            {
                KeyFieldName = "Code",
                Pager =
                {
                    PageSize = 20,
                    PageIndex = 0
                },
            };
            typeof(Measurement).GetMembers().Select(x => x.Name).ToList()
                .ForEach(n => model.Columns.Add(n));
            return CustomBinding(model);
        }

        public ActionResult GetPageResult(GridViewPagerState pagerState)
        {
            var model = GridViewExtension.GetViewModel(MEASUREMENTS_GRID_VIEW);
            model.ApplyPagingState(pagerState);
            return CustomBinding(model);
        }

        public ActionResult GetFilterResult(GridViewFilteringState filteringState)
        {
            var model = GridViewExtension.GetViewModel(MEASUREMENTS_GRID_VIEW);
            model.ApplyFilteringState(filteringState);
            return CustomBinding(model);
        }

        public ActionResult GetSortResult(GridViewColumnState columnState)
        {
            var model = GridViewExtension.GetViewModel(MEASUREMENTS_GRID_VIEW);
            model.ApplySortingState(columnState);
            return CustomBinding(model);
        }
        
        public ActionResult CustomBinding(GridViewModel model)
        {
            var co = CriteriaOperator.Parse(model.FilterExpression);
            string criterias = CriteriaToWhereClauseHelper.GetMsSqlWhere(co, true);
            var sortOptions = model.SortedColumns.Select(c => new SortOption
            {
                Field = c.FieldName, SortIndex = c.SortIndex, SortOrder = c.SortOrder.ToString()
            });

            var page = QualityControl.GetMeasurementsPage(
                model.Pager.PageIndex,
                model.Pager.PageSize,
                criterias,
                sortOptions);
            
            model.ProcessCustomBinding(
                e => e.DataRowCount = page.TotalItems,
                e => e.Data = page.Items);
            return PartialView("_ContactsGridView", model);
        }
    }
}