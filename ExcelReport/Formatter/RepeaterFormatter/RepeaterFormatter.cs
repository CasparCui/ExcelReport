﻿/*
 类：RepeaterFormatter
 描述：Repeater（元素）格式化器
 编 码 人：韩兆新 日期：2015年04月11日
 修改记录：

*/

using System;
using System.Collections.Generic;
using System.Drawing;
namespace ExcelReport
{
    public class RepeaterFormatter<TSource> : ElementFormatter
    {
        #region 成员字段
        private Point _startTagCell;
        private Point _endTagCell;
        private IEnumerable<TSource> _dataSource;
        private List<RepeaterCellInfo<TSource>> _cellInfoList;
        #endregion

        #region 构造函数
        public RepeaterFormatter(Point startTagCell, Point endTagCell, IEnumerable<TSource> dataSource, params RepeaterCellInfo<TSource>[] cellInfos)
        {
            _startTagCell = startTagCell;
            _endTagCell = endTagCell;
            _dataSource = dataSource;
            _cellInfoList = new List<RepeaterCellInfo<TSource>>();
            if (null != cellInfos && cellInfos.Length > 0)
            {
                _cellInfoList.AddRange(cellInfos);
            }
        }
        #endregion

        #region 格式化操作
        public override void Format(SheetFormatterContext context)
        {
            context.ClearRowContent(_startTagCell.X);
            context.ClearRowContent(_endTagCell.X);
            if (null == _cellInfoList || _cellInfoList.Count <= 0 || null == _dataSource)
            {
                return;
            }
            var itemCount = 0;
            foreach (TSource itemSource in _dataSource)
            {
                if (itemCount++ > 0)
                {
                    context.CopyRows(_startTagCell.X, _endTagCell.X);  //追加空行
                }
                foreach (RepeaterCellInfo<TSource> cellInfo in _cellInfoList)
                {
                    var rowIndex = context.GetCurrentRowIndex(cellInfo.CellPoint.X);
                    var row = context.Sheet.GetRow(rowIndex) ?? context.Sheet.CreateRow(rowIndex);
                    var cell = row.GetCell(cellInfo.CellPoint.Y) ?? row.CreateCell(cellInfo.CellPoint.Y);
                    SetCellValue(cell, cellInfo.DgSetValue(itemSource));
                }
            }
        }
        #endregion

        #region 添加重复单元格信息
        public void AddCellInfo(RepeaterCellInfo<TSource> cellInfo)
        {
            _cellInfoList.Add(cellInfo);
        }

        public void AddCellInfo(Point cellPoint, Func<TSource, object> dgSetValue)
        {
            _cellInfoList.Add(new RepeaterCellInfo<TSource>(cellPoint,dgSetValue));
        }
        #endregion
    }
}
