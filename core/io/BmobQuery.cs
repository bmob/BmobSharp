using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using cn.bmob.tools;
using cn.bmob.Extensions;

namespace cn.bmob.io
{
    /// <summary>
    /// Bmob查询条件对象类
    /// </summary>
    public class BmobQuery : BmobObject
    {

        /// <summary>
        /// 过滤前面num条数据
        /// </summary>
        private int skip;

        /// <summary>
        /// Skip查询语法，跳过一定的（skip个）记录数，主要用于翻页用途
        /// </summary>
        /// <param name="skip">跳过的记录数</param>
        /// <returns>返回当前对象</returns>
        public BmobQuery Skip(int skip)
        {
            this.skip = skip;
            return this;
        }

        /// <summary>
        /// keys 查询指定列
        /// </summary>
        private List<String> keys;

        /// <summary>
        /// Select查询语法，选择指定的列
        /// </summary>
        /// <param name="keys">需要获取的列</param>
        /// <returns>返回当前对象</returns>
        public BmobQuery Select(params String[] keys)
        {
            this.keys = BmobArrays.wrap(keys);
            return this;
        }

        /// <summary>
        /// 是否返回记录总数, 默认不返回！
        /// </summary>
        private bool count;

        /// <summary>
        /// count查询语法，用于获取记录总数
        /// </summary>
        /// <returns>返回当前对象</returns>
        public BmobQuery Count()
        {
            this.count = true;
            return this;
        }

        /// <summary>
        /// 只查max条数据
        /// </summary>
        private int? limit;

        /// <summary>
        /// Limit查询语法，用于获取限定数量（limit）的记录列表
        /// 
        /// 默认10条 最多1000条。地理位置查询，默认是100，最大是1000
        /// </summary>
        /// <param name="limit">记录数限制数</param>
        /// <returns>返回当前对象</returns>
        public BmobQuery Limit(int limit)
        {
            if (limit > 1000)
            {
                BmobDebug.E("设置的查询数据量 " + limit + " 将不会生效, 单次查询最大获取数量1000条. 查询数据量置为1000.");
                this.limit = 1000;
            }
            else
            {
                this.limit = limit;
            }
            return this;
        }

        private List<QueryOrder> fOrderList = new List<QueryOrder>();

        private BmobQuery order(String column, int sc)
        {
            QueryOrder order = new QueryOrder();
            order.Column = column;
            order.SC = sc;

            fOrderList.Add(order);

            return this;
        }

        /// <summary>
        /// 升序
        /// OrderBy查询语法，对查询结果进行顺序排列
        /// </summary>
        /// <param name="column">字段</param>
        /// <returns>返回当前对象</returns>
        public BmobQuery OrderBy(String column)
        {
            return order(column, 1);
        }

        /// <summary>
        /// OrderByDescending查询语法，对查询结果进行逆序排列
        /// </summary>
        /// <param name="column">字段</param>
        /// <returns>返回当前对象</returns>
        public BmobQuery OrderByDescending(String column)
        {
            return order(column, -1);
        }

        /// <summary>
        /// ThenBy查询语法，对查询结果进行顺序排列
        /// </summary>
        /// <param name="column">字段</param>
        /// <returns>返回当前对象</returns>
        public BmobQuery ThenBy(String column)
        {
            return order(column, 1);
        }

        /// <summary>
        /// ThenByDescending查询语法，对查询结果进行逆序排列
        /// </summary>
        /// <param name="column">字段</param>
        /// <returns>返回当前对象</returns>
        public BmobQuery ThenByDescending(String column)
        {
            return order(column, -1);
        }

        private String include;

        /// <summary>
        /// include查询语法，关联查询，该方法用在字段为Pointer类型时（例："Post.user"）
        /// </summary>
        /// <param name="pointer">指针对应的object字段名， 如user</param>
        /// <returns>返回当前对象</returns>
        public BmobQuery Include(string pointer)
        {
            this.include = pointer;
            return this;
        }

        private Where _where = new Where();

        public IBmobWritable where
        {
            get { return this._where; }
        }

        /// <summary>
        /// WhereContainedIn查询语法，查询某字段（column）的值包含在XX范围（values）内的数据
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="column">字段</param>
        /// <param name="values">值</param>
        /// <returns>返回当前对象</returns>
        public BmobQuery WhereContainedIn<T>(String column, params T[] values)
        {
            _where.Composite(column, "$in", values);
            return this;
        }

        /// <summary>
        /// WhereNotContainedIn查询语法，获取某一字段不是values值的数据
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="column">字段</param>
        /// <param name="values">值</param>
        /// <returns>返回当前对象</returns>
        public BmobQuery WhereNotContainedIn<T>(String column, params T[] values)
        {
            _where.Composite(column, "$nin", values);
            return this;
        }

        /// <summary>
        /// WhereEqualTo查询语法，查询字段（column）值为value的数据
        /// </summary>
        /// <param name="column">字段</param>
        /// <param name="value">值</param>
        /// <returns>返回当前对象</returns>
        public BmobQuery WhereEqualTo(String column, object value)
        {
            _where.EqualTo(column, value);
            return this;
        }

        /// <summary>
        /// WhereNotEqualTo查询语法，获取某一字段值不为value值的数据
        /// </summary>
        /// <param name="column">字段</param>
        /// <param name="value">值</param>
        /// <returns>返回当前对象</returns>
        public BmobQuery WhereNotEqualTo(String column, object value)
        {
            _where.Composite(column, "$ne", value);
            return this;
        }

        public BmobQuery WhereContainsAll<T>(String column, params T[] values)
        {
            if (values.Length == 1)
            {
                _where.EqualTo(column, values[0]);
            }
            else
            {
                _where.Composite(column, "$all", values);
            }
            return this;
        }

        /// <summary>
        /// WhereExists查询语法，查询存在指定字段的数据
        /// </summary>
        /// <param name="column">字段</param>
        /// <returns>返回当前对象</returns>
        public BmobQuery WhereExists(String column)
        {
            _where.Composite(column, "$exists", true);
            return this;
        }

        /// <summary>
        /// WhereNotExists查询语法，查询不存在指定字段的数据
        /// </summary>
        /// <param name="column"></param>
        /// <returns>返回当前对象</returns>
        public BmobQuery WhereNotExists(String column)
        {
            _where.Composite(column, "$exists", false);
            return this;
        }

        /// <summary>
        /// WhereGreaterThan查询语法，查询某字段值大于某一数的数据
        /// </summary>
        /// <param name="column">字段</param>
        /// <param name="value">值</param>
        /// <returns>返回当前对象</returns>
        public BmobQuery WhereGreaterThan(String column, object value)
        {
            _where.Composite(column, "$gt", value);
            return this;
        }

        /// <summary>
        /// WhereGreaterThanOrEqualTo查询语法，查询某字段大于等于某一数的数据
        /// </summary>
        /// <param name="column">字段</param>
        /// <param name="value">值</param>
        /// <returns>返回当前对象</returns>
        public BmobQuery WhereGreaterThanOrEqualTo(String column, object value)
        {
            _where.Composite(column, "$gte", value);
            return this;
        }

        /// <summary>
        /// WhereLessThan查询语法，查询某字段小于某一数的数据
        /// </summary>
        /// <param name="column">字段</param>
        /// <param name="value">值</param>
        /// <returns>返回当前对象</returns>
        public BmobQuery WhereLessThan(String column, object value)
        {
            _where.Composite(column, "$lt", value);
            return this;
        }

        /// <summary>
        /// WhereLessThanOrEqualTo查询语法，查询某字段小于等于某一数的数据
        /// </summary>
        /// <param name="column">字段</param>
        /// <param name="value">值</param>
        /// <returns>返回当前对象</returns>
        public BmobQuery WhereLessThanOrEqualTo(String column, object value)
        {
            _where.Composite(column, "$lte", value);
            return this;
        }

        /// <summary>
        /// WhereMatches查询语法，查询某字段的正则表达满足value的数据
        /// </summary>
        /// <param name="column">字段</param>
        /// <param name="value">正则表达式</param>
        /// <returns>返回当前对象</returns>
        public BmobQuery WhereMatches(String column, String value)
        {
            _where.Composite(column, "$regex", Regex.Escape(value));
            return this;
        }

        /// <summary>
        /// WhereStartsWith查询语法，查询column字段以value字符串开头的数据
        /// </summary>
        /// <param name="column">字段</param>
        /// <param name="value">值</param>
        /// <returns>返回当前对象</returns>
        public BmobQuery WhereStartsWith(String column, string value)
        {
            WhereMatches(column, "^" + Regex.Escape(value));
            return this;
        }

        /// <summary>
        /// WhereMatchesKeyInQuery查询语法，查询的对象中的某个列符合另一个查询
        /// 
        /// 如果你想要获取对象, 这些对象的一个字段指向的对象是符合另一个查询的.
        /// 举例说, 假设你有一个帖子(Post)类和一个评论(Comment)类, 每个评论(Comment)都有一个指向它的帖子(Post)的关系Key名为post，并且类型为Pointer, 可以找到所有带有图片的帖子(Post)的评论(Comment)
        /// 
        /// 注意默认的 limit 是 100 而且最大的 limit 是 1000，这个限制同样适用于内部的查询, 所以对于较大的数据集你可能需要细心地构建查询来获得期望的行为。
        /// 
        /// ！！查询 关联字段满足条件 的记录。
        /// 'where={"post":{"$inQuery":{"where":{"image":{"$exists":true}},"className":"Post"}}}' \
        /// https://api.bmob.cn/1/classes/Comment
        /// </summary>
        /// <param name="anotherTable">类名对应为Bmob服务端的表名</param>
        /// <param name="anotherQuery">column指向另一个类型的过滤条件</param>
        public BmobQuery WhereMatchesKeyInQuery(String column, BmobQuery anotherQuery, String anotherTable)
        {
            _where.Composite(column, "$inQuery", new Dictionary<String, object>() { { "where", anotherQuery.where }, { "className", anotherTable } });
            return this;
        }

        /// <summary>
        /// WhereRelatedTo查询语法
        /// 
        /// 如果你想获取的对象，是其父对象的关系 Relation 类型的Key的所有成员的话.
        /// 假设你有一个帖子(Post)类和一个系统默认的用户(_User)类, 而每一个帖子(Post)都可以被不同的用户(_User)所喜欢。 如果帖子(Post)类下面有一个Key名为likes，且是 Relation 类型, 存储了喜欢这个帖子(Post)的用户(_User)。那么你可以找到喜欢过同一个指定的帖子(Post)的所有用户
        /// 
        /// ！！查询user表（返回数据为user表记录），条件关联Post表的likes字段。（查询B表，关联关系在A表中）下面的例子 为获取 指定Post:1dafb9ed9b喜欢字段likes列表 的 用户详细信息
        /// 'where={"$relatedTo":{"object":{"__type":"Pointer","className":"Post","objectId":"1dafb9ed9b"},"key":"likes"}}' \
        /// https://api.bmob.cn/1/users
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="column">注意, 这里的column是Relation类型字段名， Relation字段所在表与Pointer的className是同一个表！</param>
        /// <param name="pointer">指向帖子记录的对象</param>
        /// <returns>返回当前对象</returns>
        public BmobQuery WhereRelatedTo<T>(String column, BmobPointer<T> pointer) where T : BmobTable
        {
            _where.EqualTo("$relatedTo", new Dictionary<String, object>() { { "key", column }, { "object", pointer } });
            return this;
        }

        /// <summary>
        /// WhereMatchesKeyNotInQuery查询语法，查询的某个列不符合另一个查询
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="column">字段</param>
        /// <param name="value">另一个查询</param>
        /// <returns>返回当前对象</returns>
        public BmobQuery WhereMatchesKeyNotInQuery<T>(String column, BmobQuery value) where T : BmobTable
        {
            var tempInstance = Activator.CreateInstance<T>();
            WhereMatchesKeyNotInQuery(column, value, tempInstance.table);
            return this;
        }

        /// <summary>
        /// WhereMatchesKeyNotInQuery查询语法，查询的某个列不符合另一个查询
        /// </summary>
        /// <param name="column">字段</param>
        /// <param name="value">另一个查询</param>
        /// <param name="table">表名</param>
        /// <returns>返回当前对象</returns>
        public BmobQuery WhereMatchesKeyNotInQuery(String column, BmobQuery value, String table)
        {
            _where.Composite(column, "$notInQuery", new Dictionary<String, object>() { { "where", value.where }, { "className", table } });
            return this;
        }

        /// <summary>
        /// WhereMatchesQuery查询语法，查询的对象中的某个列符合另一个指针值
        /// </summary>
        /// <typeparam name="T">泛型</typeparam>
        /// <param name="column">字段</param>
        /// <param name="value">指针</param>
        /// <returns>返回当前对象</returns>
        public BmobQuery WhereMatchesQuery<T>(String column, BmobPointer<T> value) where T : BmobTable
        {
            _where.EqualTo(column, value);
            return this;
        }

        /// <summary>
        /// 查询条件某个列值要匹配另一个查询的返回值
        /// 
        /// 举例有一个队伍(Team)保存了每个城市的得分情况且用户表中有一列为用户家乡(hometown), 您可以创建一个查询来寻找用户的家乡是得分大于0.5的城市的所有运动员
        /// 
        /// 'where={"hometown":{"$select":{"query":{"className":"Team","where":{"winPct":{"$gt":0.5}}},"key":"city"}}}' \
        /// https://api.bmob.cn/1/users
        /// </summary>
        public BmobQuery WhereMatchesFromSelect(String column, BmobQuery mQuery, String mTable, String mColumn)
        {
            _where.EqualTo(
                column,
                new Dictionary<String, object>() 
                {
                    { 
                        "$select", 
                        new Dictionary<String, object>() { 
                            { 
                                "query", 
                                new Dictionary<String, object>() { 
                                    { "className", mTable }, 
                                    { "where", mQuery } 
                                } 
                            }, 
                            { "key", mColumn } 
                        } 
                    } 
                }
            );
            return this;
        }

        /// <summary>
        /// 查询条件某个列值要匹配另一个查询的返回值
        /// 
        /// 查询Team中得分小于等于0.5的城市的所有运动员
        /// </summary>
        public BmobQuery WhereMatchesNotFromSelect(String column, BmobQuery mQuery, String mTable, String mColumn)
        {
            _where.EqualTo(
                column,
                new Dictionary<String, object>() 
                {
                    { 
                        "$dontselect", 
                        new Dictionary<String, object>() { 
                            { 
                                "query", 
                                new Dictionary<String, object>() { 
                                    { "className", mTable }, 
                                    { "where", mQuery } 
                                } 
                            }, 
                            { "key", mColumn } 
                        } 
                    } 
                }
            );
            return this;
        }

        /// <summary>
        /// WhereNear查询语法，查询最接近某一地点的数据
        /// </summary>
        /// <param name="column">字段（必须是GeoPoint坐标类型的字段）</param>
        /// <param name="geo">坐标点</param>
        /// <returns>返回当前对象</returns>
        public BmobQuery WhereNear(String column, BmobGeoPoint geo)
        {
            _where.Composite(column, "$nearSphere", geo);
            return this;
        }

        // 为了限定搜索的最大举例范围，需要加入$maxDistanceInMiles和$maxDistanceInKilometers或者$maxDistanceInRadians参数来限定.比如要找的半径在10公里内的话
        /// <summary>
        /// WhereWithinDistance查询语法，查询多少公里之内的数据
        /// </summary>
        /// <param name="column">字段</param>
        /// <param name="geo">坐标点</param>
        /// <param name="maxDistanceInKilometers">最大的公里数</param>
        /// <returns>返回当前对象</returns>
        public BmobQuery WhereWithinDistance(String column, BmobGeoPoint geo, double maxDistanceInKilometers)
        {
            _where.Composite(column, "$maxDistanceInKilometers", maxDistanceInKilometers);
            _where.Composite(column, "$nearSphere", geo);
            return this;
        }

        /// <summary>
        /// WhereWithinGeoBox查询语法，查询一个矩形范围内的信息
        /// </summary>
        /// <param name="column">字段</param>
        /// <param name="southwest">矩形左下角的坐标点</param>
        /// <param name="northeast">矩形右上角的坐标点 </param>
        /// <returns>返回当前对象</returns>
        public BmobQuery WhereWithinGeoBox(String column, BmobGeoPoint southwest, BmobGeoPoint northeast)
        {
            _where.Composite(column, "$within", new Dictionary<String, object>() { { "$box", BmobArrays.wrap(southwest, northeast) } });
            return this;
        }

        /// <summary>
        /// Or查询语法，或查询
        /// 复合查询，返回新对象
        /// 
        /// 如： where={"$or":[{"wins":{"$gt":150}},{"wins":{"$lt":5}}]}
        /// </summary>
        /// <param name="querys">查询参数</param>
        /// <returns>返回当前对象</returns>
        public BmobQuery Or(params BmobQuery[] querys)
        {
            var orWhere = new Where();

            List<IBmobWritable> ors = new List<IBmobWritable>(querys.Length + 1);
            ors.Add(this._where);
            foreach (BmobQuery cd in querys)
            {
                ors.Add(cd.where);
            }

            orWhere.addAll("$or", ors);

            this._where = orWhere;
            return this;
        }

        public BmobQuery And(params BmobQuery[] querys)
        {
            var andWhere = new Where();

            List<IBmobWritable> ands = new List<IBmobWritable>(querys.Length + 1);
            ands.Add(this._where);
            foreach (BmobQuery cd in querys)
            {
                ands.Add(cd.where);
            }

            andWhere.addAll("$and", ands);

            this._where = andWhere;
            return this;
        }



        /// <summary>
        /// 查询条件序列化对象
        /// 
        /// 将请求封装为JSON发送到Bmob服务器中
        /// </summary>
        /// <param name="output"></param>
        /// <param name="all">是否输出所有的字段的值！请求的json是all为false。用于toString！</param>
        public override void write(BmobOutput output, Boolean all)
        {
            if (this.include != null)
                output.Put("include", this.include);
            if (this.count)
                output.Put("count", this.count);
            if (this.skip != 0)
                output.Put("skip", this.skip);
            if (this.limit != null)
                output.Put("limit", this.limit);

            if (keys != null && keys.Count > 0)
                output.Put("keys", this.keys.join());
            if (fOrderList.Count > 0)
                output.Put("order", fOrderList.join());
            if (_where != null && _where.Length() > 0)
                output.Put("where", _where);
        }

        /// <summary>
        /// 浅拷贝 
        /// </summary>
        public BmobQuery clone()
        {
            var res = new BmobQuery();
            res.include = this.include;
            res.count = this.count;
            res.skip = this.skip;
            res.limit = this.limit;

            res._where = this._where;

            if (this.keys != null)
            {
                res.keys = new List<String>();
                res.keys.AddRange(this.keys);
            }

            if (this.fOrderList != null)
            {
                res.fOrderList = new List<QueryOrder>();
                res.fOrderList.AddRange(this.fOrderList);
            }

            return res;
        }

    }

    /// <summary>
    /// 排序
    /// </summary>
    internal struct QueryOrder
    {
        /// <summary>
        /// 字段
        /// </summary>
        public String Column { get; set; }
        /// <summary>
        /// -1 降 / 1 升
        /// </summary>
        public int SC { get; set; }

        public override string ToString()
        {
            return (this.SC > 0 ? "" : "-") + this.Column;
        }
    }

    /// <summary>
    /// 条件查询语句where类
    /// </summary>
    internal class Where : BmobObject
    {
        private IDictionary real = new Dictionary<String, Object>();

        public int Length()
        {
            return real.Count;
        }

        public Where EqualTo(String column, object value) { BmobOutput.Save(real, column, value); return this; }

        internal Where Composite(String column, String type, Object value)
        {
            BmobOutput.Composite(real, column, type, value);
            return this;
        }

        internal Where addAll(String column, List<IBmobWritable> value)
        {
            real.Add(column, value);
            return this;
        }

        public override void write(BmobOutput output, bool all)
        {
            base.write(output, all);

            var enumerator = real.GetEnumerator();
            while (enumerator.MoveNext())
            {
                var key = (string)enumerator.Key;
                var value = enumerator.Value;

                output.Put(key, value);
            }

        }

    }
}
