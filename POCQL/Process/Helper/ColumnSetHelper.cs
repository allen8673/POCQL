using POCQL.Model;
using POCQL.Model.MapAttribute;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using POCQL.Process.Helper;
using POCQL.ToolExt;

namespace POCQL.Process.Helper
{
    internal static class ColumnSetHelper
    {
        /// <summary>
        /// 取得物件Property對應的Column資訊
        /// </summary>
        /// <param name="obj">全域物件(Domain Model)</param>
        /// <param name="ignoreValue">
        ///     是否忽略Property Value；
        ///     如果不忽略，當Value等於Null時不會取對應的Column資訊
        /// </param>
        /// <param name="mapProperty">
        ///     是否轉換結果的名字，如果要，最後會以{Column} AS {Property Name}輸出結果
        /// </param>
        /// <returns></returns>
        public static IEnumerable<ColumnSet> GetColumnSets<T>(T obj, bool ignoreValue, bool mapProperty)
            where T : class
        {
            // *** 檢核Domain Model是否符合規範 ***
            return obj.ObjectAuth(entityAttr => obj.ColumnSetListProcess(obj.GetType().GetProperties(), entityAttr.MainTable, ignoreValue, mapProperty));
        }

        /// <summary>
        /// 取得物件Property對應的Column資訊
        /// </summary>
        /// <param name="domainObj">全域物件(Domain Model)</param>
        /// <param name="viewObj">區域物件(Veiw Model)</param>
        /// <param name="ignoreValue">
        ///     是否忽略Property Value；
        ///     如果不忽略，當Value等於Null時不會取對應的Column資訊
        /// </param>
        /// <param name="mapProperty">
        ///     是否轉換結果的名字，如果要，最後會以{Column} AS {Property Name}輸出結果
        /// </param>
        /// <param name="otherDomainProps">區域物件以外，額外指定的全域物件Property</param>
        /// <returns></returns>
        public static IEnumerable<ColumnSet> GetColumnSets<TDomain, TView>(TDomain domainObj, TView viewObj, bool ignoreValue, bool mapProperty)
            where TDomain : class
        {
            // *** 檢核Domain Model是否符合規範 ***
            return domainObj.ObjectAuth(entityAttr =>
            {
                List<ColumnSet> columns = new List<ColumnSet>();
                // *** 取得Domain Model的Property資訊 ***
                PropertyInfo[] domainProps = domainObj.GetType().GetProperties();
                PropertyInfo[] viewProps = viewObj.GetType().GetProperties();

                var mustAppendProp = domainObj is IMustAppend<TDomain> ?
                                     new Func<IEnumerable<string>>(() =>
                                     {
                                         return ((IMustAppend<TDomain>)domainObj).MustAppendItem().ParsePropertyName();
                                     })()
                                     : new string[] { };

                return domainObj.ColumnSetListProcess(domainProps.Where(dp => viewProps.Any(vp => vp.Name == dp.Name) || mustAppendProp.Any(mp => dp.Name == mp)),
                                                      entityAttr.MainTable, ignoreValue, mapProperty).ToArray();
            });
        }

        /// <summary>
        /// 處理ColumnSet清單程序
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="obj">目標物件，通常為Domain或是View Model</param>
        /// <param name="propInfos">Property Infos</param>
        /// <param name="defaultTable">預設Table</param>
        /// <param name="ignoreValue">
        ///     是否忽略Property Value；
        ///     如果不忽略，當Value等於Null時不會取對應的Column資訊
        /// </param>
        /// <param name="mapProperty">
        ///     是否轉換結果的名字，如果要，最後會以{Column} AS {Property Name}輸出結果
        /// </param>
        /// <returns></returns>
        private static IEnumerable<ColumnSet> ColumnSetListProcess<T>(this T obj, IEnumerable<PropertyInfo> propInfos, string defaultTable, bool ignoreValue, bool mapProperty)
            where T : class
        {
            ColumnPropertyInfo columnPropInfo;

            // ***  取得NULL Able 白名單 ***
            var nullableProperties = obj is INullable ?
                                     new Func<IEnumerable<string>>(() =>
                                     {
                                         return ((INullable)obj).NullableProperties ?? new List<string>();
                                     })() : new string[] { };

            foreach (PropertyInfo propInfo in propInfos)
            {
                // *** 解析Property所擁有與Column有關的設定 ***
                columnPropInfo = new ColumnPropertyInfo(propInfo, defaultTable, propInfo.GetValue(obj), nullableProperties);

                // *** 如果沒有主要的ColumnMapperAttribute設定就直接跳過 ***
                if (!columnPropInfo.HasColumnSetting) continue;

                // *** 如果Column Name有指定資料來源，即:attr.Column格式為{Column}:{DataSource}，
                //     則直接設定Column的資料來源，而不會取得Property的值 ***
                if (columnPropInfo.Column.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).Count() > 1)
                {
                    yield return ColumnSet.ParseDataSource(columnPropInfo.Column, columnPropInfo.Table, string.Empty,
                                                      mapProperty ? columnPropInfo.Name : string.Empty,
                                                      columnPropInfo.IsPrimaryKey,
                                                      columnPropInfo.ReadOnly,
                                                      columnPropInfo.IsAggregate,
                                                      columnPropInfo.MapType);
                    continue;
                }

                // *** 如果不忽略Value = Null的條件下，
                //     Property Value等於Null就跳過    ***
                if (!ignoreValue && !columnPropInfo.Nullable && columnPropInfo.Value == null) continue;

                #region Final Return
                yield return mapProperty ?
                             new ColumnSet(columnPropInfo.Column,
                                           columnPropInfo.Name,
                                           columnPropInfo.Table,
                                           columnPropInfo.Value,
                                           columnPropInfo.AggregateOperator,
                                           columnPropInfo.IsPrimaryKey,
                                           columnPropInfo.ReadOnly, columnPropInfo.IsAggregate,
                                           columnPropInfo.MapType) :
                             new ColumnSet(columnPropInfo.Column,
                                           string.Empty,
                                           columnPropInfo.Table,
                                           columnPropInfo.Value,
                                           columnPropInfo.AggregateOperator,
                                           columnPropInfo.IsPrimaryKey,
                                           columnPropInfo.ReadOnly,
                                           columnPropInfo.IsAggregate,
                                           columnPropInfo.MapType);
                #endregion
            }
        }

        /// <summary>
        /// 依指定Match值，取得與Match值相似的Column資訊；
        /// 此方法只適用於具有'MultiColumnMapperAttribute'設定的Property
        /// </summary>
        /// <param name="obj">全域物件(Domain Model)</param>
        /// <param name="ignoreValue">
        ///     是否忽略Property Value；
        ///     如果不忽略，當Value等於Null時不會取對應的Column資訊
        /// </param>
        /// <param name="matches">指定Match值</param>
        /// <returns></returns>
        public static IEnumerable<ColumnSet> GetMatchColumnSets<T>(T obj, bool ignoreValue, params string[] matches)
            where T : class
        {
            // *** 檢核Domain Model是否符合規範 ***
            return obj.ObjectAuth(entityAttr =>
            {
                List<ColumnSet> columns = new List<ColumnSet>();

                MultiColumnMapperAttribute attr;
                object propValue = null;
                string columnName;

                foreach (var prop in obj.GetType().GetProperties())
                {
                    // *** 如果有客製設定，先判斷專案本身是否符合客製設定 ***
                    // *** 如果目前客製不在Attribute的客製設定中就不理會 ***
                    //CustomizeAttribute customSetting = prop.GetCustomAttribute<CustomizeAttribute>(true) as CustomizeAttribute;
                    //if (customSetting != null && !customSetting.Customizations.Contains(ToolGlobalEnum.Customize ?? string.Empty))
                    //    continue;

                    // *** 是否有 MultiColumnMapperAttribute ***
                    attr = prop.GetCustomAttributes(typeof(MultiColumnMapperAttribute), true).FirstOrDefault()
                           as MultiColumnMapperAttribute;

                    //找不到則跳過
                    if (attr == null) continue;

                    // *** 判斷是否是為Primary Key ***
                    bool isPk = (prop.GetCustomAttribute<PrimaryKeyAttribute>(true) as PrimaryKeyAttribute) != null;

                    // *** 如果被設定為ReadOnly，只要跟寫入有關的動作(INSERT/UPDATE)都會被忽略 ***
                    bool readOnly = (prop.GetCustomAttribute<ReadOnlyAttribute>(true) as ReadOnlyAttribute) != null;

                    // *** 判斷是否是為Aggregate欄位 ***
                    bool aggregate = (prop.GetCustomAttribute<AggregationAttribute>(true) as AggregationAttribute) != null;

                    foreach (string matche in matches)
                    {
                        columnName = attr.Columns.Where(c => c.StartsWith(matche)).FirstOrDefault();

                        //欄位名稱起始字串沒有符合 match 值 
                        if (columnName == null) continue;

                        // *** 如果有指定DataSource的話，就直接設定Column的資料來源，不用接下來的動作 ***
                        if (!attr.DataSource.IsNullOrEmpty())
                        {
                            columns.Add(ColumnSet.ParseDataSource($"{columnName}:{attr.DataSource}", string.Empty, string.Empty, prop.Name, isPk, readOnly, aggregate));
                            continue;
                        }

                        // *** 如果Column Name有指定資料來源，即:attr.Column格式為{Column}:{DataSource}，
                        //     則直接設定Column的資料來源，而不會取得Property的值 ***
                        if (columnName.Split(new char[] { ':' }, StringSplitOptions.RemoveEmptyEntries).Count() > 1)
                        {
                            columns.Add(ColumnSet.ParseDataSource(columnName, string.Empty, string.Empty, prop.Name, isPk, readOnly, aggregate));
                            continue;
                        }


                        if (!ignoreValue)
                        {
                            propValue = prop.GetValue(obj);
                            //Property 值為 Null，跳過
                            if (propValue == null) continue;
                        }

                        columns.Add(new ColumnSet(columnName, prop.Name, null, propValue, isPk, readOnly, aggregate));
                    }

                }
                return columns;
            });


        }
    }
}
