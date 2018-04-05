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
如果Employee所對應的表單會隨著程序而移動，則可以將`table`參數化，也就是任意參數名寫入`{#...#}`中，設定方式下面章節說明。

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
## Entity Model:
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
## Condition Model:
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
### .Columns\<T>(bool mapToProp = true) & <br/>.Columns\<TDomain, TView>(bool mapToProp = true) ###
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
> 一傳入參數`columns`指定所要查詢的欄位

|參數|型態|說明|
|:--|:--|:--|
| columns | params string[] | 指定要查詢之欄位，若要給予欄位別名，字串格式為`{欄位}:{別名}`。 |
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

以`Employee`和`User`兩個類別

## 進階方法
### .Where\<T>(T conditionObj, [ConditionBinder/ConditionBinder\<T>] binder)
> 以條件物件給予查詢條件，同時透過`ConditionBinder`於特殊應用下給予指定條件

|參數|型態|說明|
|:--|:--|:--|
| conditionObj | T | 查詢用物件 |
| binder | ConditionBinder /<br/>ConditionBinder\<T> | 用以比對Property並將值映射至對應查詢條件上的類別(物件)。 |

### .Columns\<TDomain>(ColumnBinder\<TDomain> binder) & <br/> .Columns\<TDomain, TView>(ColumnBinder\<TDomain, TView> binder)
> 1. 透過`ColumnBinder`指定欲查詢的資料，並將值指定至`TDomain`類型中的特定Property
> 2. 如同`.Columns<TDomain, TView>(bool mapToProp = true)`，比對`View Model`與`Domain Model(Entity)`交集的Property後，再透過`ColumnBinder`將指定查詢的資料映射至特定Property。

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






