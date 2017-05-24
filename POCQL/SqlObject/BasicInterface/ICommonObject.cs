using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace POCQL.SqlObject
{
    interface ICommonObject<TDerive> : IBaseObject<TDerive>
    {
        /// <summary>
        /// 從指定物件(Domain Model)設定Column資訊
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="value">INSERT/UPDATE欄位的資料，限定為有設定ColumnMapperAttribute的Domain Model</param>
        /// <param name="otherColumns">
        ///     其他指定的欄位與資料來源的對應；
        ///     資料格式為: {Column}:{DataSource};
        ///     Ex: CRT_DT:GETDATE()
        /// </param>
        /// <returns></returns>
        TDerive Columns<T>(T value, params string[] otherColumns)
            where T : class;

        /// <summary>
        /// 從指定欄位對應資料來源字串(格式:{Column}:{DataSource})設定Column資訊
        /// </summary>
        /// <param name="columns">
        ///     指定Column和其對應的資料來源；
        ///     資料格式為: {Column}:{DataSource};
        ///     Ex: CRT_DT:GETDATE()
        /// </param>
        /// <returns></returns>
        TDerive Columns(params string[] columns);

        /// <summary>
        /// 從指定Match值設定Column資訊
        /// </summary>
        /// <param name="value">INSERT/UPDATE欄位的資料，限定為有設定ColumnMapperAttribute的Domain Model</param>
        /// <param name="matches">指定的Match值</param>
        /// <returns></returns>
        TDerive MatcheColumns<T>(T value, params string[] matches)
            where T : class;
    }
}
