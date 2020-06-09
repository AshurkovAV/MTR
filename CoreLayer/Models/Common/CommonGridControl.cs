using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DevExpress.Xpf.Grid;

namespace Medical.CoreLayer.Models.Common
{
    public class CommonGridControl<T>
    {
        static public List<T> _selectTerritoryAccount = new List<T>();
        static public List<T> _selectTerritoryEvent = new List<T>();
        static public List<T> _selectEconomicAccount = new List<T>();
        static public GridControl GridEvent = new GridControl();

        private List<T> GetSelectedGridRows(GridControl gridControl)
        {
            var selectGrid = new List<T>();
            var handles = gridControl.GetSelectedRowHandles();
            if (handles == null || !handles.Any()) return null;

            handles = handles.Where(x => x >= 0).Select(x => x).ToArray();
            var count = handles.Length;

            for (int index = 0; index < count; index++)
            {
                int i = index;
                var handle = handles[i];
                var row = gridControl.GetRow(handle);

                if (row is DevExpress.Data.NotLoadedObject)
                {
                    gridControl.GetRowAsync(handle).ContinueWith(x =>
                    {
                        selectGrid.Add((T)x.Result);
                    });
                }
                else
                    selectGrid.Add((T)row);
            }
            return selectGrid;
        }


        /// <summary>
        /// Возвращаем в статичесую переменную выбранные счета с территорий
        /// </summary>
        /// <param name="gridControl"></param>
        public void GridRowsTerAccount(GridControl gridControl)
        {
            _selectTerritoryAccount = GetSelectedGridRows(gridControl);
        }

        /// <summary>
        /// Возвращаем в статичесую переменную выбранные случаи счета с территории
        /// </summary>
        /// <param name="gridControl"></param>
        public void GridRowsEvent(GridControl gridControl)
        {
            _selectTerritoryEvent = GetSelectedGridRows(gridControl);
            //_selectTerritoryAccount.Clear();
            //var handles = gridControl.GetSelectedRowHandles();
            //if (handles == null || !handles.Any()) return null;

            //handles = handles.Where(x => x >= 0).Select(x => x).ToArray();
            //var count = handles.Length;
            //object[] obj = new object[count];

            //for (int index = 0; index < count; index++)
            //{
            //    int i = index;
            //    var handle = handles[i];
            //    var row = gridControl.GetRow(handle);

            //    if (row is DevExpress.Data.NotLoadedObject)
            //    {
            //        gridControl.GetRowAsync(handle).ContinueWith(x =>
            //        {
            //            _selectTerritoryAccount.Add((T)x.Result);
            //        });
            //    }
            //    else
            //        _selectTerritoryAccount.Add((T)row);
            //}
            //return obj;
        }

        /// <summary>
        /// Возвращаем в статичесую переменную выбранные счета с территорий
        /// </summary>
        /// <param name="gridControl"></param>
        public void GridRowsEconomicAccount(GridControl gridControl)
        {
            _selectEconomicAccount = GetSelectedGridRows(gridControl);
        }


    }
}
