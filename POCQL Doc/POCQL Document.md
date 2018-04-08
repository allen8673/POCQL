# POCQL 簡介
>POCQL為提供使用者方法，藉由POCO/Entity對應SQL欄位，
>以及ViewModel產生必要的CRUD Sql語法;
>同時利用[Dapper](https://github.com/StackExchange/Dapper)達成ORM的功能。
>需要注意的是，POCQL本身僅只有產生SQL語法的功能，並不具有與DB連線的功能；
>與DB連線並實現CRUD的功能仍然需要依賴實作`IDbConnection`的物件。

# 大綱

[toc]

<p style="page-break-before: always;"/>
# POCQL Attributes
## 1. EntityMapperAttribute
> 定義Entity為有效Mapper類別，以及該Entity，如果用於映射的Entity並無掛上該Attribute則會被視為是違反映射物件。

|參數|說明|
|:--|:--|
| table | 定義Entity所對應的Table。 |

```cs
[EntityMapper("EMPLOYEE")]
public class Employee
{
	...
}
```
指定`Employee`類別對應名為`EMPLOYEE`的表單

```cs
[EntityMapper("{#TABLE_NAME#}")]
public class Employee
{
	...
}
```
如果Employee所對應的表單會隨著程序而變動，則可以將`table`參數化，也就是任意參數名寫入`{#...#}`中，使用方式會於[`.SetTable`](#toc_43)說明。

<p style="page-break-before: always;"/>
## 2. ColumnMapperAttribute
> 定義Entity(類別)的Property所對應的Table的欄位。於有效Mapper類別中無被掛上該Attribute，在映射時會被忽略。

|參數|說明|
|:--|:--|
| column | 定義Property所對應的欄位。|
| table | 定義Property所對應的Table；如果未給值則以EntityMapperAttribute的table所指定的表單為主。 |

```cs
[EntityMapper("EMPLOYEE")]
public class Employee
{
	[ColumnMapper("NAME")]
	public string Name {get; set;}
	
	[ColumnMapper("ADDRESS", "EMPLOYEE_DETAIL")]
	public string Address {get; set;}
	
	...
}
```
指定`Employee`類別對應名為`EMPLOYEE`的表單，其中Property`Name`對應`EMPLOYEE`的欄位`NAME`，而`Address`則是對應名為`EMPLOYEE_DETAIL`的表單的欄位`ADDRERSS`。

```cs
[EntityMapper("EMPLOYEE")]
public class Employee
{
	[ColumnMapper("NAME")]
	public string Name {get; set;}
	
	[ColumnMapper("MDF_DT : GETDATE()")]
	public DateTime? ModifyDate {get; set;}
	
	...
} 
```
`ColumnMapperAttribute`的`column`可以透過`{Column}:{Data Source}`的格式自動針對特定欄位於INSER/UPDATE時寫入指定資料來源值；<br/>
以上例說明，`Employee`的Property`ModifyDate`對應於表單`EMPLOYEE`的欄位`MDF_DT`，同時被指定於INSERT/UPDATE時，寫入資料皆由Sql函式`GETDATE()`來。

<p style="page-break-before: always;"/>
## 3. MultiColumnMapperAttribute
> 用於單一Property需要對應於多欄位的情況。

|參數|說明|
|:--|:--|
| columns | 指定對應於該Property的所有欄位。 |

```cs
[EntityMapper]
public class BaseEmployee
{
	[MultiColumnMapper("EMP_NAME", "CLE_NAME", "MNG_NAME")]
	public string Name {get; set;}
	
	...
} 
```
指定通用類別`BaseEmployee`的Property`Name`同時為`BaseEmployee`，`CLE_NAME`和`MNG_NAME`所映射。

```cs
[EntityMapper]
public class BaseEmployee
{
	[MultiColumnMapper("EMP_NAME", "CLE_NAME", "MNG_NAME",)]
	public string Name {get; set;}
	
	[MultiColumnMapper("EMP_MDF_DT", "CLE_MDF_DT", "MNG_MDF_DT : GETDATE()")]
	public DateTime? ModifyDate {get; set;}
	
	...	
} 
```
如同`ColumnMapperAttribute`，可以`{Column}:{Data Source}`的格式透過`MultiColumnMapper`的`columns`，指定特定欄位於INSERT/UPDATE時寫入欄位的資料來源；<br/>
以上例說明，`BaseEmployee`的Property`ModifyDate`被設定為如果`ModifyDate`映射`MNG_MDF_DT`時，則寫入欄位的資料一律由Sql函式`GETDATE()`來。

```cs
[EntityMapper]
public class BaseEmployee
{
	[MultiColumnMapper("EMP_NAME", "CLE_NAME", "MNG_NAME",)]
	public string Name {get; set;}
	
	[MultiColumnMapper("{:GETDATE():}", "EMP_MDF_DT", "CLE_MDF_DT", "MNG_MDF_DT")]
	public DateTime? ModifyDate {get; set;}
	
	...	
}
```
亦可以`{:Data Source:}`的格式透過`columns`設定所有映射該Property的欄位於INSERT/UPDATE時寫入欄位的資料來源；<br/>
以上例說明，`ModifyDate`以`{:GETDATE():}`被設定不論是哪一個欄位，於INSERT/UPDATE資料來源皆為Sql函式`GETDATE()`。

```cs
[EntityMapper]
public class BaseEmployee
{
	[MultiColumnMapper("EMP_NAME", "CLE_NAME", "MNG_NAME",)]
	public string Name {get; set;}
	
	[MultiColumnMapper("{:DataSource_1:}", "EMP_MDF_DT", "CLE_MDF_DT", "MNG_MDF_DT:DataSource_2")]
	public DateTime? ModifyDate {get; set;}
	
	...	
}
```
注意：如果當`{Column}:{Data Source}`和`{:Data Source:}`同時並存時，被以格式`{Column}:{Data Source}`設定的欄位會先以自己的設定（資料來源）為主；<br/>
以上例說明，`ModifyDate`同時以`{Column}:{Data Source}`和`{:Data Source:}`格式被設定於INSERT/UPDATE時寫入欄位的資料來源，當`ModifyDate`映射`MNG_MDF_DT`時，寫入欄位的資料來源為`DataSource_2`；當映射其餘欄位時，寫入欄位的資料來源則一律為`DataSource_1`。

<p style="page-break-before: always;"/>
## 4. PrimaryKeyAttribute
> 定義Property為Table的Primary Key；
> 掛上該Attribute的Property於INSERT會將Property值寫入欄位，但是於UPDATE時則會忽略該欄位的值。

|參數|說明|
|:--|:--|
| autoIncrement | 定義該Primary Key於DB是否自動增長；如果為預設值`false`，則INSERT時會將該Property值寫入欄位；如果值為`true`，則INSERT時就不會將該Property值寫入欄位。 |

```cs
[EntityMapper("EMPLOYEE")]
public class Employee
{
	[PrimaryKey, ColumnMapper("ID")]
	public string Id {get; set;}

	[ColumnMapper("NAME")]
	public string Name {get; set;}
		
	...
} 
```
定義`Employee`類別的Property`Id`對應表單`EMPLOYEE`的Primary Key欄位`ID`，由於`PrimaryKeyAttribute`的`autoIncrement`為`false`，所以當INSERT資料時，`Id`的值會寫入欄位`ID`中。

<p style="page-break-before: always;"/>
## 5. ReadOnlyAttribute
> 定義Property只能用於讀取對應欄位資訊，不能用於寫入/更新資訊。

```cs
[EntityMapper("EMPLOYEE")]
public class Employee
{	
	[PrimaryKey, ColumnMapper("ID")]
	public string Id {get; set;}

	[ColumnMapper("NAME")]
	public string Name {get; set;}
	
	[ColumnMapper("AGE"), ReadOnly]
	public int Age {get; set;}
	
	...
} 
```
定義`Employee`類別的Property`Age`對應表單`EMPLOYEE`的欄位`AGE`，並且`Age`只能用來讀取`AGE`的資料，其值卻不能於INSERT/UPDATE時寫入`AGE`。

<p style="page-break-before: always;"/>
## 6. NullableAttribute
> 定義Property即使值為NULL，仍可以將該值(NULL)寫入欄位，或用於查詢。 

```cs
[EntityMapper("EMPLOYEE")]
public class Employee
{	
	[PrimaryKey, ColumnMapper("ID")]
	public string Id {get; set;}

	[ColumnMapper("NAME")]
	public string Name {get; set;}
	
	[ColumnMapper("AGE"), ReadOnly]
	public int Age {get; set;}
	 
	[ColumnMapper("PHONE"), Nullable]
	public string Phone {get; set;}
	
	...
} 
```
定義`Employee`類別的Property`Phone`對應表單`EMPLOYEE`的欄位`PHONE`；當INSERT/UPDATE時，掛上`ColumnMapperAttribute`的Property值如果為NULL，則會被忽略不處理；以上例而言，`Phone`被定義為Nullable，所以於INSERT/UPDATE時，即使其值為NULL，仍然會將值(NULL)寫入欄位`PHONE`中。

<p style="page-break-before: always;"/>
## 7. AggregationAttribute
> 定義Property所對應結果為彙總欄位。

```cs
[EntityMapper("CUSTOMER_ORDER")]
public class CustomerOrder
{	
	[PrimaryKey, ColumnMapper("ID")]
	public string ID {get; set;}

	[ColumnMapper("NAME")]
	public string Name {get; set;}
	
	[ColumnMapper("GOODS")]
	public string Goods {get; set;}
	
	[ColumnMapper("PRICE"), Aggregation( AggregateFunction.SUM)]
	public string TotalPrice {get; set;}
	
	...
} 
```
```cs
SqlSet sql = Select.Columns<CustomerOrder>()                   .From("CUSTOMER_ORDER")                   .Where("ORDER_DT < GETDATE()")                   .Output();
```
定義`CustomerOrder`類別中的`TotalPrice`Property，Select結果為彙總函式`SUNM`的結果。一旦類別中有掛上`AggregationAttribute`的Property，則其餘Property於SQL中會自動被GROUP BY。
以上述類別`CustomerOrder`為例，以`Select`方法所取得的SQL如下：

```sql
SELECT ID AS [ID],
       NAME AS [Name],
       GOODS AS [Goods],
       SUM(PRICE) AS [TotalPrice] 
       ...
  FROM [CUSTOMER_ORDER] WITH (NOLOCK) 
 WHERE ORDER_DT < GETDATE() 
 GROUP BY ID, NAME, GOODS, ...
```

<p style="page-break-before: always;"/>
## 8. ConditionAttribute
> 用於條件物件，定義Entity(類別)的Property所對應的條件欄位。

```cs
[EntityMapper("EMPLOYEE")]
public class EmployeeCondition
{	
	[Condition("NAME")]
	public string Name {get; set;}
	
	[Condition("AGE")]
	public int? Age {get; set;}
	 
	[Condition("PHONE")]
	public string Phone {get; set;}
	
	...
} 
```
定義`EmployeeCondition`類別為用於查訓的條件物件，並將其Property掛上`ConditionAttribute`，若其中Preperty值不為NULL，則查詢條件會依設定附加(Append)至查詢條件上。

<p style="page-break-before: always;"/>
## 9. BetweenSetAttribute
> 用於條件物件，定義Property為BETWEEN條件式。

```cs
public class IntervalVal<T>{
	[BetweenSet(BetweenSet.PrefixValue)]
	public T PrefixValue { get; set; }
	
	[BetweenSet(BetweenSet.PostfixValue)]
	public T PostfixValue { get; set; }
}

[EntityMapper("EMPLOYEE")]
public class EmployeeCondition
{	
	[Condition("NAME")]
	public string Name {get; set;}
	
	[Condition("AGE")]
	public IntervalVal<int?> Age {get; set;}
	 
	[Condition("PHONE")]
	public string Phone {get; set;}
	
	...
} 
```

定義`IntervalVal`類別為用於查詢條件`BETWEEN`值，並且用於`EmployeeCondition`類別中Property`Age`。一旦`Age`為NULL，或是`Age`本身不為NULL，但是其中`PrefixValue`和`PostfixValue`皆為NULL，則該條件不會被附加至查詢條件；如果`Age`其中`PrefixValue`或`PostfixValue`之一為NULL，則會自動以條件式`>=`或`<=`取代`BETWEEN`。僅有當`Age`的`PrefixValue`及`PostfixValue`皆有值的情況，才會附加`BETWEEN`條件式至查詢條件上。

<p style="page-break-before: always;"/>
# Example Model
定義以下Model以作為接下來的範例類型。<br/>
## Entity Model
```cs
/// <summary>/// 員工資訊/// </summary>
[EntityMapper("EMPLOYEE")]
public class Employee
{
	[PrimaryKey, ColumnMapper("EMPLOYEE_NO")]
	public string EmployeeNo { get; set; }

	[ColumnMapper("NAME")]
	public string Name { get; set; }

	[ColumnMapper("SEX")]
	public string Sex { get; set; }

	[ColumnMapper("DEPT")]
	public string Department { get; set; }

	[ColumnMapper("TITLE")]
	public string Title { get; set; }
	
	public bool IsSupervisor { get; set; }
}```
```cs/// <summary>
/// 員工詳細資料
/// </summary>
[EntityMapper("USER")]
public class User
{
	[PrimaryKey, ColumnMapper("ID")]
	public string Id { get; set; }

	[ColumnMapper("EMPLOYEE_NO")]
	public string EmployeeNo { get; set; }

	[ColumnMapper("NAME")]
	public string Name { get; set; }

	[ColumnMapper("SEX")]
	public string Sex { get; set; }

	[ColumnMapper("BIRTH_DATE")]
	public DateTime? BirthDate { get; set; }

	[ColumnMapper("MARITAL_STATUS")]
	public string MaritalStatus { get; set; }

	[ColumnMapper("ADDRESS")]
	public string Address { get; set; }

	[ColumnMapper("PHONE_NUMBER")]
	public string PhoneNumber { get; set; }}
```
<p style="page-break-before: always;"/>
## Condition Model
```cs
/// <summary>/// 員工資訊查詢Model/// </summary>
[EntityMapper("EMPLOYEE")]
public class EmployeeCdt
{
	[Condition("EMPLOYEE_NO")]
	public string EmployeeNo { get; set; }

	[Condition("NAME")]
	public string Name { get; set; }

	[Condition("SEX")]
	public string Sex { get; set; }

	[Condition("DEPT")]
	public string Department { get; set; }

	[Condition("TITLE")]
	public string Title { get; set; }
}
```
```cs
/// <summary>
/// 員工詳細資料查詢model
/// </summary>
[EntityMapper("USER")]
public class UserCdt
{
	[ColumnMapper("EMPLOYEE_NO")]
	public string EmployeeNo { get; set; }

	[ColumnMapper("NAME")]
	public string Name { get; set; }

	[ColumnMapper("SEX")]
	public string Sex { get; set; }

	[ColumnMapper("BIRTH_DATE")]
	public Interval BirthDate { get; set; }

	[ColumnMapper("MARITAL_STATUS")]
	public string MaritalStatus { get; set; }

	[ColumnMapper("ADDRESS")]
	public string Address { get; set; }

	[ColumnMapper("PHONE_NUMBER")]
	public string PhoneNumber { get; set; }}
```

<p style="page-break-before: always;"/>
# Select Method
## 常用方法

### .From(string table, [string tableAlias,] [bool lockTable])
> 指定查詢表單(Table)

|參數|型態|說明|
|:--|:--|:--|
| table | string | 查詢表單 |
| tableAlias | string | 選填欄位；查詢表單別名。 |
| lockTable | bool | 選填欄位；是否Lock表單，**是**-產出`SELECT ... FROM [Table]`Sql，**否**-產出`SELECT ... FROM [Table] WITH (ONLOCK)`Sql；預設為*false*。 |

### .Where(string condition)
> 以字串給予查詢條件 

|參數|型態|說明|
|:--|:--|:--|
| condition | string | Sql查詢條件 |
### .Where\<T>(T conditionObj)
> 以條件物件給予查詢條件

|參數|型態|說明|
|:--|:--|:--|
| conditionObj | T | 查詢用物件 |
### .Columns\[\<T>\\\<TDomain, TView>\](bool mapToProp = true)
> 1. 依類型指定所要查詢的欄位。
> 2. 依`View Model`與`Domain Model(Entity)`比對出交集的Property後，指定所要查詢的欄位。

|參數|型態|說明|
|:--|:--|:--|
| mapToProp | bool | 是否將欄位映射至Property；**是**-以`{Column} AS {Property}`，**否**-以`{Column} AS {Column}`輸出查詢欄位語法。|

**基本使用範例**<br/>
查詢`Employee`類別對應欄位範例及Sql結果如下：

```cs
string sql = Select.Columns<Employee>()                   .From("EMPLOYEE")
                   .Where("NAME = @name")
                   .ToString();
```
```sql
SELECT EMPLOYEE_NO AS [EmployeeNo],
       NAME AS [Name],
       SEX AS [Sex],
       DEPT AS [Department],
       TITLE AS [Title] 
 FROM [EMPLOYEE] WITH (NOLOCK) 
WHERE NAME = @name
```
給予`EMPLOYEE`表單別名的範例及Sql結果如下：

```cs
string sql = Select.Columns<Employee>()                   .From("EMPLOYEE", "EPY")
                   .Where("EPY.NAME = @name")
                   .ToString();
```
```sql
SELECT EPY.EMPLOYEE_NO AS [EmployeeNo],
       EPY.NAME AS [Name],
       EPY.SEX AS [Sex],
       EPY.DEPT AS [Department],
       EPY.TITLE AS [Title] 
 FROM [EMPLOYEE] EPY WITH (NOLOCK) 
WHERE EPY.NAME = @name 
```
透過條件物件給予查詢條件範例及Sql結果如下：

```cs
EmployeeCdt condition = new EmployeeCdt{    Name = "User",    Department = "100"};

string sql = Select.Columns<Employee>()                   .From("EMPLOYEE", "EPY")
                   .Where(condition)
                   .ToString();
```
```sql
SELECT EPY.EMPLOYEE_NO AS [EmployeeNo],
       EPY.NAME AS [Name],
       EPY.SEX AS [Sex],
       EPY.DEPT AS [Department],
       EPY.TITLE AS [Title] 
 FROM [EMPLOYEE] EPY WITH (NOLOCK) 
WHERE EPY.NAME = @Name 
  AND EPY.DEPT = @Department 
```
### .Columns(params string[] columns)
> 傳入參數`columns`指定所要查詢的欄位

|參數|型態|說明|
|:--|:--|:--|
| columns | string[] | 指定要查詢之欄位，若要給予欄位別名，字串格式為`{欄位}:{別名}`。 |
**使用範例及Sql結果如下**

```cs
string sql = Select.Columns("EMPLOYEE_NO : No",                            "NAME",                             "SEX")                   .From("EMPLOYEE")                   .Where("DEPT = @dept")                   .ToString();
```
```sql
SELECT EMPLOYEE_NO AS [No],
       NAME AS [NAME],
       SEX AS [SEX] 
 FROM [EMPLOYEE] WITH (NOLOCK)
 WHERE DEPT = @dept 
```
上例特別以`{欄位}:{別名}`格式將`EMPLOYEE_NO`轉成別名`No`，其餘欄位則未作轉換。<br/>
參數`columns`可傳入值為0至多個；如果未傳入任何參數值，同時也無使用其他方法指定查詢欄位，則會產生`Select * ...`的Sql語法。**範例及結果如下**：

```cs
string sql = Select.Columns()                   .From("EMPLOYEE")
                   .Where("NAME = @name")
                   .ToString();
```
```sql
SELECT * 
  FROM [EMPLOYEE] WITH (NOLOCK)
 WHERE NAME = @name 
```

### .MatcheColumns\<T>(string matche)
> 用於Property掛上[`MultiColumnMapperAttribute`](#toc_5)的類別，透過參數`matche`指定查詢相似名稱的欄位。

|參數|型態|說明|
|:--|:--|:--|
| matche | string | 用以比對Property所對應欄位的相似Prefix字串 |

```cs
[EntityMapper]public class UserInfo{    [MultiColumnMapper("CRT_USER_ID", "MDF_USER_ID", "MAG_USER_ID")]    public string UserID { get; set; }    [MultiColumnMapper("CRT_USER_NAME", "MDF_USER_NAME", "MAG_USER_NAME")]    public string UserName { get; set; }    [MultiColumnMapper("CRT_DEPT_ID", "MDF_DEPT_ID", "MAG_DEPT_ID")]    public string DeptID { get; set; }    [MultiColumnMapper("CRT_DEPT_NAME", "MDF_DEPT_NAME", "MAG_DEPT_NAME")]    public string DeptName { get; set; }
}
```
**以上類別作為範例程式及結果如下**

```cs
string sql = Select.MatcheColumns<UserInfo>("MDF")
                   .From("EMPLOYEE")
                   .Where("DEPT = @Dept")
                   .ToString();
```
```sql
SELECT MDF_USER_ID AS [UserID],
       MDF_USER_NAME AS [UserName],
       MDF_DEPT_ID AS [DeptID],
       MDF_DEPT_NAME AS [DeptName]
  FROM [EMPLOYEE] WITH (NOLOCK)
 WHERE DEPT = @Dept 
```

### .\[InnerJoin/LeftJoin](string table, [string tableAlias,] [bool lockScrTable,] string joinCondition)
> 產生`INNSER/LEFT JOIN`Sql語法。 

|參數|型態|說明|
|:--|:--|:--|
| table | string | JOIN 表單 |
| tableAlias | string | JOIN 表單別名 |
| lockScrTable | bool | 是否Lock Join表單 |
| joinCondition | string | JOIN條件 |

```cs
[EntityMapper("EMPLOYEE")]public class EmployeeForJoin{    [PrimaryKey, ColumnMapper("EMPLOYEE_NO")]
    public string EmployeeNo { get; set; }
    
    [ColumnMapper("NAME")]
    public string Name { get; set; }
    
    [ColumnMapper("SEX")]
    public string Sex { get; set; }
    
    [ColumnMapper("DEPT")]
    public string Department { get; set; }
    
    [ColumnMapper("TITLE")]
    public string Title { get; set; }
   
    [ColumnMapper("ID", "USER")]
    public string Id { get; set; }
    
    [ColumnMapper("BIRTH_DATE", "USER")]
    public DateTime? BirthDate { get; set; }
    
    [ColumnMapper("MARITAL_STATUS", "USER")]
    public string MaritalStatus { get; set; }
    
    [ColumnMapper("ADDRESS", "USER")]
    public string Address { get; set; }
    
    [ColumnMapper("PHONE_NUMBER", "USER")]
    public string PhoneNumber { get; set; }}
```
以上`EmployeeForJoin`為例，使用範例及結果如下：

```cs
string sql = Select.Columns<EmployeeForJoin>()                   .From("EMPLOYEE", "EPY")                   .InnerJoin("USER", "USR", "EPY.EMPLOYEE_NO = USR.EMPLOYEE_NO")                   .Where("DEPT = @Dept")                   .ToString();
```
```sql
SELECT EPY.EMPLOYEE_NO AS [EmployeeNo],
       EPY.NAME AS [Name],
       EPY.SEX AS [Sex],
       EPY.DEPT AS [Department],
       EPY.TITLE AS [Title],
       USR.ID AS [Id],
       USR.BIRTH_DATE AS [BirthDate],
       USR.MARITAL_STATUS AS [MaritalStatus],
       USR.ADDRESS AS [Address],
       USR.PHONE_NUMBER AS [PhoneNumber] 
  FROM [EMPLOYEE] EPY WITH (NOLOCK)
 INNER JOIN [USER] USR WITH (NOLOCK) 
    ON EPY.EMPLOYEE_NO = USR.EMPLOYEE_NO
 WHERE DEPT = @Dept
```

#### 以多類別(Entity)產生JOIN語法
> 上述範例使用類別`EmployeeForJoin`是將兩張表單的的欄位集中於同一類別中，但是SELECT同時支援以不同Entity產生JOIN語法

以`Employee`和`User`兩個類別產生JOIN語法範例及結果如下：

```cs
string sql = Select.Columns<Employee>()
                   .Columns<User>()                   .From("EMPLOYEE", "EPY")                   .InnerJoin("USER", "USR", "EPY.EMPLOYEE_NO = USR.EMPLOYEE_NO")                   .Where("DEPT = @Dept")                   .ToString();
```
```sql
SELECT EPY.EMPLOYEE_NO AS [EmployeeNo],
       EPY.NAME AS [Name],
       EPY.SEX AS [Sex],
       EPY.DEPT AS [Department],
       EPY.TITLE AS [Title],
       USR.ID AS [Id],
       USR.BIRTH_DATE AS [BirthDate],
       USR.MARITAL_STATUS AS [MaritalStatus],
       USR.ADDRESS AS [Address],
       USR.PHONE_NUMBER AS [PhoneNumber] 
  FROM [EMPLOYEE] EPY WITH (NOLOCK)
 INNER JOIN [USER] USR WITH (NOLOCK) 
    ON EPY.EMPLOYEE_NO = USR.EMPLOYEE_NO
 WHERE DEPT = @Dept
```

### .SubQuery<DType, VType>(string select, Expression<Func<DType, object>> propertyMap)
> 比對`View Model`與`Domain Model(Entity)`交集的Property，並且藉由字串給予子查詢，將結果映射至指定Property。

|參數|型態|說明|
|:--|:--|:--|
| select | string | 子查詢字串。 |
| propertyMap | Expression<Func<DType, object>> | 指定映射的Property。 |

於`Employee`新增Property`ID`；**方法使用範例及結果如下**

```cs
string sql = Select.Columns<Employee, T>()                   .SubQuery<Employee, T>(@"SELECT ID 
                                              FROM USER USR 
                                             WHERE USR.EMPLOYEE_NO = EMPLOYEE_NO", 
                                          i => i.Id)                   .From("EMPLOYEE")                   .Where("DEPT = @Dept")                   .ToString();
```
```sql
SELECT EMPLOYEE_NO AS [EmployeeNo],
       NAME AS [Name],S
       EX AS [Sex],
       DEPT AS [Department],
       TITLE AS [Title],
       (SELECT ID FROM USER USR WHERE USR.EMPLOYEE_NO = EMPLOYEE_NO) AS [Id] 
  FROM [EMPLOYEE] WITH (NOLOCK)
 WHERE DEPT = @Dept 
```

### .SubQuery\[\<T>\\\<DType, VType>](SelectObject select, Expression propertyMap )
> 透過`SelectObject`(`Select`方法)給予子查詢，並將結果映射至指定Property。

|參數|型態|說明|
|:--|:--|:--|
| select | SelectObject | 子查詢`Select`物件。 |
| propertyMap | Expression<Func<DType, object>> | 指定映射的Property。 |

**方法使用範例及結果如下**

```cs
string sql = Select.Columns<Employee, T>()                   .SubQuery<Employee, T>(Select.Columns("ID")                                                .From("USER", "USR")                                                .Where("USR.EMPLOYEE_NO = EMPLOYEE_NO"),                                           i => i.Id)                   .From("EMPLOYEE")                   .Where("DEPT = @Dept")                   .ToString();
```
```sql
SELECT EMPLOYEE_NO AS [EmployeeNo],
       NAME AS [Name],
       SEX AS [Sex],
       DEPT AS [Department],
       TITLE AS [Title],
       (SELECT ID AS [ID] 
          FROM [USER] USR WITH (NOLOCK) 
         WHERE USR.EMPLOYEE_NO = EMPLOYEE_NO ) AS [Id] 
  FROM [EMPLOYEE] WITH (NOLOCK) 
 WHERE DEPT = @Dept 
```

## 其他常用方法
> 以下整理其他常用方法的使用範例及產生Sql結果

### .Top(int top)
> 產生`SELECT TOP n ... FROM ...`語法

|參數|型態|說明|
|:--|:--|:--|
| top | int | 查詢Top數量 |

```cs
string sql = Select.Top(5)                   .Columns<Employee>()                                                  .From("EMPLOYEE")
```
```sql
SELECT TOP 5 
       EMPLOYEE_NO AS [EmployeeNo],
       NAME AS [Name],
       SEX AS [Sex],
       DEPT AS [Department],
       TITLE AS [Title]   FROM [EMPLOYEE] WITH (NOLOCK)
 WHERE DEPT = @Dept 
```
### .Distinct()
> 產生`SELECT Distinct ... FROM ...`語法

```cs
string sql = Select.Distinct()                   .Columns<Employee>()                   .From("EMPLOYEE")                   .Where("DEPT = @Dept")                   .ToString();
```
```sql
SELECT DISTINCT         EMPLOYEE_NO AS [EmployeeNo],
       NAME AS [Name],
       SEX AS [Sex],
       DEPT AS [Department],
       TITLE AS [Title]   FROM [EMPLOYEE] WITH (NOLOCK) WHERE DEPT = @Dept 
```

### .OrderBy(string orderBy, [string defaultOrder,] [string sort])
> 產生`SELECT ... ORDER BY ...`語法

|參數|型態|說明|
|:--|:--|:--|
| orderBy | string | ORDER BY 欄位或是Property Name。 | 
| sort | string | Sort類型，ASC或是DESC。 | 

```cs
string sql = Select.Columns<Employee>()                   .From("EMPLOYEE")                   .Where("DEPT = @Dept")                   .OrderBy("EMPLOYEE_NO")                   .ToString();
```
```sql
SELECT EMPLOYEE_NO AS [EmployeeNo],
       NAME AS [Name],
       SEX AS [Sex],
       DEPT AS [Department],
       TITLE AS [Title]   FROM [EMPLOYEE] WITH (NOLOCK)  WHERE DEPT = @Dept  ORDER BY EMPLOYEE_NO ASC 
```
### .GroupBy(params string[] groupBy)
> 產生 `SELECT ... GROUP BY ...`語法。如果所要Select類別有掛上[`AggregationAttribute`](#toc_9)，也會產生`GROUP BY`語法。

|參數|型態|說明|
|:--|:--|:--|
| groupBy | string[] | 0到多個 GROUP BY 欄位或是Property Name。 | 

```cs
string sql = Select.Columns<Employee>()                   .From("EMPLOYEE")                   .Where("DEPT = @Dept")                   .GroupBy("EMPLOYEE_NO", "NAME")                   .ToString();
```
```sqlSELECT EMPLOYEE_NO AS [EmployeeNo],
       NAME AS [Name],
       SEX AS [Sex],
       DEPT AS [Department],
       TITLE AS [Title]   FROM [EMPLOYEE] WITH (NOLOCK) WHERE DEPT = @Dept  GROUP BY EMPLOYEE_NO, NAME
```

### .Paging(int pageRow, int page)
> 產生分頁語法；注意，必須與`OrderBy()`方法一起使用

|參數|型態|說明|
|:--|:--|:--|
| pageRow | int | 每個分頁的資料量。 |
| page | int | 第幾頁資料。 |

```cs
string sql = Select.Columns<Employee>()                   .From("EMPLOYEE")                   .Where("DEPT = @Dept")                   .OrderBy("EMPLOYEE_NO")                   .Paging(50,2)                   .ToString();
```
```sql
SELECT EMPLOYEE_NO AS [EmployeeNo],
       NAME AS [Name],
       SEX AS [Sex],
       DEPT AS [Department],
       TITLE AS [Title]   FROM [EMPLOYEE] WITH (NOLOCK) WHERE DEPT = @Dept ORDER BY EMPLOYEE_NO ASC OFFSET 50 ROWS FETCH FIRST 50 ROWS ONLY 
```
## 輸出結果常用方法
### .Output([object otherParam])
> 輸出包含Sql以及所需參數的類型`SqlSet`物件。

```cs
SqlSet sqlset = Select.Columns<Employee>()                      .From("EMPLOYEE")                      .Where("DEPT = '100'")                      .Output();
```
```cs
SqlSet sqlset = Select.Columns<Employee>()                      .From("EMPLOYEE")                      .Where("DEPT = @Dept")                      .Output(new {                          Dept = "100"                      });
```

### .Output\<T>([object otherParam])
> 泛型輸出包含Sql以及所需參數的物件；其中泛型類別需要包含與`SqlSet`相同的Property型態及名稱。

```cs
public class MySqlSet{    public int Index { get; set; } 
    public string Sql { get; private set; }
    public object Parameters { get; private set; }}
```
以上類別`MySqlSet`為例：

```cs
MySqlSet sqlset = Select.Columns<Employee>()                        .From("EMPLOYEE")                        .Where("DEPT = '100'")                        .Output<MySqlSet>();
```
```cs
MySqlSet sqlset = Select.Columns<Employee>()                        .From("EMPLOYEE")                        .Where("DEPT = @Dept")                        .Output<MySqlSet>(new                        {                            Dept = "100"                        });
```

### .Query\<T>([object parameters,] [DataAccess trans = null])
> 查詢結果，並輸出泛型`IEnumerable`型別物件

```cs
IEnumerable<Employee> result = Select.Columns<Employee>()                                     .From("EMPLOYEE")                                     .Where("DEPT = '100'")                                     .Query<Employee>();
```
```cs
IEnumerable<Employee> result = Select.Columns<Employee>()                                     .From("EMPLOYEE")                                     .Where("DEPT = @Dept")                                     .Query<Employee>(new                                     {                                         Dept = "100"                                     });
```

### .PagingQuery\<T>([object parameters,] [DataAccess trans = null])
> 查詢分頁結果，並輸出泛型`PagingResult`物件型別；注意，該方法需要與[`.OrderBy`](#toc_30)和[`.Paging`](#toc_32)一起使用

```cs
PagingResult<Employee> result = Select.Columns<Employee>()                                      .From("EMPLOYEE")                                      .Where("DEPT = '100'")                                      .OrderBy("EMPLOYEE_NO")                                      .Paging(50, 2)                                      .PagingQuery<Employee>();
```
```cs
PagingResult<Employee> result = Select.Columns<Employee>()                                      .From("EMPLOYEE")                                      .Where("DEPT = @Dept")                                      .OrderBy("EMPLOYEE_NO")                                      .Paging(50, 2)                                      .PagingQuery<Employee>(new
                                      {
                                          Dept = "100"
                                      });
```
### .PagingQuery\<TData, TResult>([object parameters,] Func\<PagingResult\<TData>, TResult> map, [DataAccess trans = null])
> 查詢分頁結果，並將泛型`PagingResult`物件結果映射至自訂型別上。

```cs
public class MyPagingResult{    public IEnumerable<object> QueryDatas{ get; set; }    public int DataCount { get; set; }}
```
以上類別`MyPagingResult`為例：

```cs
MyPagingResult result = Select.Columns<Employee>()                              .From("EMPLOYEE")                              .Where("DEPT = '100'")                              .OrderBy("EMPLOYEE_NO")                              .Paging(50, 2)                              .PagingQuery<Employee, MyPagingResult>(i=>new MyPagingResult                              {                                  QueryDatas = i.Datas,                                  DataCount = i.Total                              });
```
```cs
MyPagingResult result = Select.Columns<Employee>()                              .From("EMPLOYEE")                              .Where("DEPT = @Dept")                              .OrderBy("EMPLOYEE_NO")                              .Paging(50, 2)                              .PagingQuery<Employee, MyPagingResult>(new                              {                                  Dept = "100"                              },                              i => new MyPagingResult                              {                                  QueryDatas = i.Datas,                                  DataCount = i.Total                              });
```
## 進階方法
### .Where\<T>(T conditionObj, [ConditionBinder/ConditionBinder\<T>] binder)
> 以條件物件給予查詢條件，同時透過`ConditionBinder`於特殊應用下給予指定條件

|參數|型態|說明|
|:--|:--|:--|
| conditionObj | T | 查詢用物件 |
| binder | ConditionBinder /<br/>ConditionBinder\<T> | 用以比對Property並將值映射至對應查詢條件上的類別(物件)。 |

### .Columns\[\<TDomain>\\\<TDomain, TView>](ColumnBinder\[\<TDomain>\\\<TDomain, TView>] binder)
> 1. 透過`ColumnBinder`指定欲查詢的資料，並將值指定至`TDomain`類型中的特定Property
> 2. 比對`View Model`與`Domain Model(Entity)`交集的Property後，再透過`ColumnBinder`將指定查詢的資料映射至特定Property。

|參數|型態|說明|
|:--|:--|:--|
| binder | ColumnBinder\<TDomain> /<br/>ColumnBinder\<TDomain, TView> | 用以將指定查詢資料映射至指定Property的類別(物件) |

**方法使用範例如下**

```cs
string sql = Select.Columns<Employee>()
                    .Columns<Employee>(new ColumnBinder<Employee>
                    {
                        { "KIND" },
                        { "CASE WHEN KIND = '1' THEN 1 ELSE 0 END", i => i.IsSupervisor },
                    })
                    .From("EMPLOYEE")
                    .Where("DEPT = @Dept")
                    .ToString();
```
```sql
SELECT EMPLOYEE_NO AS [EmployeeNo],
       NAME AS [Name],
       SEX AS [Sex],
       DEPT AS [Department],
       TITLE AS [Title],
       KIND AS [KIND],
       CASE WHEN KIND = '1' THEN 1 ELSE 0 END AS [IsSupervisor] 
  FROM [EMPLOYEE] WITH (NOLOCK)
 WHERE DEPT = @Dept 
```
執得注意，上例並未將欄位`KIND`映射至任何Property，以致Sql查詢該欄位是以原本`查詢資料`(以該範例是KIND)字串為別名；
而上例將指定將`CASE WHEN KIND = '1' THEN 1 ELSE 0 END`結果映射至`IsSupervisor`，所以另該查詢結果別名為`IsSupervisor`。

### .Cte(string cte)
> 產生CTE Sql

|參數|型態|說明|
|:--|:--|:--|
| cte | string | Cte內容 |

```cs
SelectObject cteSql = Select.Columns<Employee>()                            .From("EMPLOYEE")                            .Where("DEPT = @Dept");string sql = Select.Columns()                   .Cte($";WITH CTE AS ({cteSql})")                   .From("CTE")                   .ToString();
```
```sql
;WITH CTE AS (SELECT EMPLOYEE_NO AS [EmployeeNo],
                     NAME AS [Name],SEX AS [Sex],
                     DEPT AS [Department],
                     TITLE AS [Title]                 FROM [EMPLOYEE] WITH (NOLOCK)
               WHERE DEPT = @Dept)
               SELECT  * FROM [CTE] WITH (NOLOCK)
```

### .SetTable(string tableParameter, string currentTable)
> 如同[`EntityMapperAttribute`](#toc_3)所述，類別會有所對應的表單會隨著程序而變動的情況，所以需要將類別對應表單參數化，而後於程序根據需求進行更換。

|參數|型態|說明|
|:--|:--|:--|
| tableParameter | string | 表單參數，指由`{#..#}`所包覆的字串。 |
| currentTable | string | 指定更換的表單名稱。 |

```cs
/// <summary>/// 員工資訊查詢Model/// </summary>
[EntityMapper("{#EPY_TABLE#}")]
public class Employee
{
	[PrimaryKey, ColumnMapper("EMPLOYEE_NO")]
	public string EmployeeNo { get; set; }

	[ColumnMapper("NAME")]
	public string Name { get; set; }

	[ColumnMapper("SEX")]
	public string Sex { get; set; }

	[ColumnMapper("DEPT")]
	public string Department { get; set; }

	[ColumnMapper("TITLE")]
	public string Title { get; set; }
}
```
將類別`Employee`所對應表單參數化，如上；使用範例及結果如下：

```cs
string sql = Select.Columns<Employee>()                   .SetTable("EPY_TABLE", "EMPLOYEE")                   .From("EMPLOYEE")                   .Where("DEPT = @Dept")                   .ToString();
```
```sql
SELECT EMPLOYEE_NO AS [EmployeeNo],
       NAME AS [Name],
       SEX AS [Sex],
       DEPT AS [Department],
       TITLE AS [Title]   FROM [EMPLOYEE] WITH (NOLOCK) WHERE DEPT = @Dept 
```
### 關於`UNION`
> `Select`並未提供`Union`方法，但是可以藉由[`Cte`](#toc_42)和[`SetTable`](#toc_43)兩個方法達成UNION的需求。

以[`SetTable`](#toc_43)表單參數化類別`Employee`為例：

```cs
string[] unionTables = new string[] { "EMPLOYEE", "EMPLOYEE_END", "EMPLOYEE_BACKUP" };IEnumerable<SelectObject> cteSqls =    unionTables.Select(table => Select.Columns<Employee>()                                      .SetTable("EPY_TABLE", table)                                      .From(table)                                      .Where("DEPT = @Dept"));string sql = Select.Columns()                   .Cte($";WITH CTE AS({string.Join(" UNION ", cteSqls)})")                   .From("CTE")                   .ToString();
```
```sql
;WITH CTE AS (SELECT EMPLOYEE_NO AS [EmployeeNo],
                     NAME AS [Name],
                     SEX AS [Sex],
                     DEPT AS [Department],
                     TITLE AS [Title]                 FROM [EMPLOYEE] WITH (NOLOCK)
               WHERE DEPT = @Dept
               UNION 
              SELECT EMPLOYEE_NO AS [EmployeeNo],
                     NAME AS [Name],
                     SEX AS [Sex],
                     DEPT AS [Department],
                     TITLE AS [Title]                 FROM [EMPLOYEE_END] WITH (NOLOCK)
               WHERE DEPT = @Dept 
               UNION 
              SELECT EMPLOYEE_NO AS [EmployeeNo],
                     NAME AS [Name],
                     SEX AS [Sex],
                     DEPT AS [Department],
                     TITLE AS [Title]
                FROM [EMPLOYEE_BACKUP] WITH (NOLOCK)
               WHERE DEPT = @Dept)
               SELECT * FROM [CTE] WITH (NOLOCK) 
```

# Insert Method
## 常用方法
### .Table(string table)
> 指定Insert表單

|參數|型態|說明|
|:--|:--|:--|
| table | string | Insert表單名稱 |

### .Columns(params string[] otherColumns)
> 透過參數指定要寫入的欄位及其值，通常透過`{欄位}:{資料值/來源}`指定要寫入的資料。

|參數|型態|說明|
|:--|:--|:--|
| otherColumns | string[] | 透過`{欄位}:{資料值/來源}`格式指定寫入欄位及其值 |

**使用範例及結果**

```cs
string sql = Insert.Table("EMPLOYEE")                   .Columns("NAME : '王O明'",                            "SEX : 'M'",                            "DEPT : '100'",                            "TITLE : dbo.GETDEF('title')")                   .ToString();
```
```sqlINSERT INTO [EMPLOYEE]       (NAME, SEX, DEPT, TITLE)VALUES 
       ('王O明', 'M', dbo.GETDEPT(), dbo.GETDEF('title'))
```

### .Columns\<T>(T value, params string[] otherColumns)
> 透過物件指定要寫入的欄位及其值，同時也能夠過`otherColumns`指定除物件以外要寫入的欄位及值
 
|參數|型態|說明|
|:--|:--|:--|
| value | T | 要寫入Property對應欄位的物件。 |
| otherColumns | string[] | 透過`{欄位}:{資料值/來源}`格式指定寫入欄位及其值 |

```cs
Employee employee = new Employee{    Name = "王O明",
    Sex = "M",
    Department = "100"};string sql = Insert.Table("EMPLOYEE")                   .Columns(employee,                                                        "TITLE : dbo.GETDEF('title')")                   .ToString();
```
```sql
INSERT INTO [EMPLOYEE]       (NAME, SEX, DEPT, TITLE)VALUES 
       (@NAME, @SEX, @DEPT, dbo.GETDEF('title'))
```

### .ColumnsFrom(string sourceTable, params string[] columns)
> 指定寫入欄位的資料來自於指定表單的欄位；產生 `INSERT INTO [Table] VALUES([Columns]) SELECT [Source Columns] FROM [Source Table] WHERE ...` Sql語法。

|參數|型態|說明|
|:--|:--|:--|
| sourceTable | T | 資料來源表單 |
| columns | string[] | 欄位和資料來源欄位，如果有指定資料來源以格式為`{Column}:{Source Column}`設定，例如：`NAME : USER_NAME`，否則會自動帶入相同欄位。`ColumnsFrom()`方法可以搭配`Where()`方法一起使用。 |

```cs
string sql = Insert.Table("EMPLOYEE")                   .ColumnsFrom("USER",                                 "NAME", "SEX",                                "DEPT : @DEPT", "TITLE : @TITLE",                                "DEPT_NAME : dbo.GETNAME('dept', @Department)")                   .Where("ID = @id")                   .ToString();
```
```sqlINSERT INTO [EMPLOYEE]       (NAME, SEX, DEPT, TITLE, DEPT_NAME)SELECT NAME AS [NAME],
       SEX AS [SEX],
       @DEPT AS [DEPT],
       @TITLE AS [TITLE],
       dbo.GETNAME('dept', @Department) AS [DEPT_NAME]  FROM [USER]WHERE ID = @id 
```

## 進階方法
### .Columns\<T>(ValueBinder\<T> valueBinder)
> 透過`ValueBinder`指定欄位於特定類別中的Property有值時，以格式`{欄位}:{資料值/來源}`設定寫入值。

|參數|型態|說明|
|:--|:--|:--|
| value | T | 要寫入Property對應欄位的物件。 |
| valueBinder | ValueBinder | `ValueBinder`僅依指定Property是否有值，決定是否需要將資料值寫入特定欄位。 |

```cs
Employee employee = new Employee{    Name = "王O明",
    Sex = "M",
    Department = "100"};string sql = Insert.Table("EMPLOYEE")                   .Columns(employee,                                                        "TITLE : dbo.GETDEF('title')")
                   .Columns(new ValueBinder<Employee>(employee)                   {                       { "DEPT_NAME : dbo.GETNAME('dept', @DEPT)", i => i.Department}                   })                   .ToString();
```
```sql
INSERT INTO [EMPLOYEE]       (NAME, SEX, DEPT, TITLE, DEPT_NAME)VALUES 
       (@NAME, @SEX, @DEPT, dbo.GETDEF('title'), dbo.GETNAME('dept', @DEPT))
```

### .MatcheColumns\<T>(T value, params string[] matches)
> 用於Property掛上[`MultiColumnMapperAttribute`](#toc_5)的類別，透過參數`matche`將值寫入相似名稱的欄位中。

|參數|型態|說明|
|:--|:--|:--|
| matche | string | 用以比對Property所對應欄位的相似Prefix字串 |

```cs
[EntityMapper]public class UserInfo{    [MultiColumnMapper("CRT_USER_ID", "MDF_USER_ID", "MAG_USER_ID")]    public string UserID { get; set; }    [MultiColumnMapper("CRT_USER_NAME", "MDF_USER_NAME", "MAG_USER_NAME")]    public string UserName { get; set; }    [MultiColumnMapper("CRT_DEPT_ID", "MDF_DEPT_ID", "MAG_DEPT_ID")]    public string DeptID { get; set; }    [MultiColumnMapper("CRT_DEPT_NAME", "MDF_DEPT_NAME", "MAG_DEPT_NAME")]    public string DeptName { get; set; }
}
```
**以上類別作為範例程式及結果如下**

```cs
Employee employee = new Employee{    Name = "王O明",    Sex = "M",    Department = "100"};UserInfo userinfo = new UserInfo{    UserID = "123",                UserName = "系O員",                DeptID = "001",                DeptName = "系統管理部"            };            string sql = Insert.Table("EMPLOYEE")                               .Columns(employee)                               .MatcheColumns(userinfo, "CRT")                               .Where("ID = @id")                               .ToString();
```
```sql
INSERT INTO [EMPLOYEE]       (NAME, SEX, DEPT, 
        CRT_USER_ID, CRT_USER_NAME, CRT_DEPT_ID, CRT_DEPT_NAME)VALUES 
       (@NAME, @SEX, @DEPT,
        @CRT_USER_ID, @CRT_USER_NAME, @CRT_DEPT_ID, @CRT_DEPT_NAME)
```

# Update Method
## 常用方法
### .Table(string table, [string alias])
> 指定Update表單，同時可以給予別名

|參數|型態|說明|
|:--|:--|:--|
| table | string | Update表單名稱 |
| alias | string | Update表單別名 |

### .Where()
> 使用方法與[`Select Method`](#toc_15)的`Where`方法相同，請參閱 [`.Where(string condition)`](#toc_18)， [`.Where<T>(T conditionObj)`](#toc_19) 和 [`Where<T>(T conditionObj, ConditionBinder binder)`](#toc_40)的說明；

### .Columns(params string[] otherColumns)
> 透過參數指定要更新的欄位及其值，通常透過`{欄位}:{資料值/來源}`指定要更新的資料。

|參數|型態|說明|
|:--|:--|:--|
| otherColumns | string[] | 透過`{欄位}:{資料值/來源}`格式指定更新欄位及其值 |

**使用範例及結果**

```cs
string sql = Update.Table("EMPLOYEE")                   .Columns("DEPT : '100'",                            "TITLE : 'OO專員'")                   .Where("EMPLOYEE_NO = @no")                   .ToString();
```
```sql UPDATE [EMPLOYEE]     SET DEPT = '100',
        TITLE = 'OO專員'  WHERE EMPLOYEE_NO = @no
```

### .Columns\<T>(T value, params string[] otherColumns)
> 透過物件指定要更新的欄位及其值，同時也能夠過`otherColumns`指定除物件以外要更新的欄位及值
 
|參數|型態|說明|
|:--|:--|:--|
| value | T | 要更新Property對應欄位的物件。 |
| otherColumns | string[] | 透過`{欄位}:{資料值/來源}`格式指定更新欄位及其值 |

```cs
Employee employee = new Employee{    Department = "100",    Title = "OO專員"};string sql = Update.Table("EMPLOYEE")                   .Columns(employee)                   .Where("EMPLOYEE_NO = @no")                   .ToString();
```
```sql UPDATE [EMPLOYEE]     SET DEPT = @DEPT,
        TITLE = @TITLE  WHERE EMPLOYEE_NO = @no
```

### .ColumnsFrom(string sourceTable, string sourceTableAlias, string sourceCondition, params string[] columns)
> 指定更新欄位的資料來自於指定表單的欄位；產生`UPDATE [Table] SET([Columns] = [Source Columns]) FROM [Source Table]  WHERE [CONDITION]`Sql語法

|參數|型態|說明|
|:--|:--|:--|
| sourceTable | string | 更新資料來源表單。 |
| sourceTableAlias | string | 更新資料來源表單別名。 |
| sourceCondition | string | 來源資料條件。 |
| columns | string[] | 欄位和資料來源欄位，如果有指定資料來源以格式為`{Column}:{Source Column}`設定，例如：`NAME : USER_NAME`，否則會自動帶入相同欄位。 |

```cs
string sql = Update.Table("EMPLOYEE", "EPY")                   .ColumnsFrom("USER", "USR", "USR.EMPLOYEE_NO = EPY.EMPLOYEE_NO",                                "NAME", "ADDRESS", "PHONE_NUMBER")                   .Where("EPY.EMPLOYEE_NO = @no")                   .ToString();
```
```sql
 UPDATE EPY    SET EPY.NAME = USR.NAME,
        EPY.ADDRESS = USR.ADDRESS,
        EPY.PHONE_NUMBER = USR.PHONE_NUMBER   FROM [EMPLOYEE] EPY  INNER JOIN [USER] USR 
     ON USR.EMPLOYEE_NO = EPY.EMPLOYEE_NO  WHERE EPY.EMPLOYEE_NO = @no
```




