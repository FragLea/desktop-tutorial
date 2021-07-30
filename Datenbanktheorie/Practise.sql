use Northwind
select * from Employees

select P.ProductID, P.ProductName, S.CompanyName, C.CategoryName, P.QuantityPerUnit, P.UnitPrice, P.UnitsInStock, P.Discontinued 
from Products P
inner join Suppliers S on P.SupplierID = S.SupplierID
inner join Categories C on P.CategoryID = C.CategoryID

Select P.ProductName
from Products P 
inner join Suppliers S on P.SupplierID = S.SupplierID 
inner join Categories C on P.CategoryID = C.CategoryID
where P.ProductID = 1 


select *
from Products P
inner join Suppliers S on P.SupplierID = S.SupplierID
inner join Categories C on P.CategoryID = C.CategoryID


Select SupplierID, CompanyName from Suppliers order by SupplierID

Select CategoryName from Categories

Select SupplierID from Suppliers where CompanyName = 'Exotic Liquids'	/*for inserting SupplierID into Products*/
Select CategoryID from Categories where CategoryName = 'Dairy Products'	/*For inserting CategoryID into Products*/

Update Products
set 

use Northwind
Select * from Products
select * from vw_simpleProductOverview
Select * from Suppliers
select * from Employees
select * from EmployeeTerritories
Select * from Categories




create view vw_simpleProductOverview
as 
select P.ProductID, P.ProductName, S.CompanyName, C.CategoryName, P.QuantityPerUnit, P.UnitPrice, P.UnitsInStock, P.Discontinued 
from Products P
left join Suppliers S on P.SupplierID = S.SupplierID
left join Categories C on P.CategoryID = C.CategoryID

create trigger trig_insertSimpleProductOverview
on vw_simpleProductOverview
instead of insert
as
begin
	set nocount on;
	insert into Products( 
	ProductName, 
	QuantityPerUnit, 
	UnitPrice, 
	UnitsInStock, 
	UnitsOnOrder, 
	ReorderLevel, 
	Discontinued, 
	SupplierID, 
	CategoryID)
	select 
	ProductName, 
	QuantityPerUnit, 
	UnitPrice, 
	UnitsInStock, 
	0, 
	0, 
	Discontinued, 
	(Select SupplierID from Suppliers where CompanyName = (Select CompanyName from inserted)), 
	(Select CategoryID from Categories where CategoryName = (Select CategoryName from inserted)) 
	from inserted
end

create trigger trig_deleteSimpleProductOverview
on vw_simpleProductOverview
instead of delete
as
begin
	set nocount on;
	delete from Products where ProductID = (Select ProductID from deleted)
end

create trigger trig_updateSimpleProductOverview
on vw_simpleProductOverview
instead of update
as
begin
	set nocount on;
	update Products
	set
	ProductName = (select ProductName from inserted),
	SupplierID = (Select SupplierID from Suppliers where CompanyName = (Select CompanyName from inserted)),
	CategoryID = (Select CategoryID from Categories where CategoryName = (Select CategoryName from inserted)),
	QuantityPerUnit = (select QuantityPerUnit from inserted),
	UnitPrice = (select UnitPrice from inserted), 
	UnitsInStock = (select UnitsInStock from inserted),
	Discontinued = (select Discontinued from inserted)
	where ProductID = (select ProductID from deleted)
end

Insert into vw_simpleProductOverview(ProductName, CompanyName, CategoryName, QuantityPerUnit, UnitPrice, UnitsInStock, Discontinued) values('Testprodukt2', 'Testfirma', 'Testkategorie', '69 box x 420 g', 42.00, 169, 0)
delete from vw_simpleProductOverview where ProductID = 81;
update vw_simpleProductOverview
set ProductName = 'Awesome Product',
CompanyName = 'Bigfoot Breweries', 
CategoryName = 'Seafood',
QuantityPerUnit = 'infinite', 
UnitPrice = 0,
UnitsInStock = '85',
Discontinued = 0
where ProductID = 84

"Insert into vw_simpleProductOverview" +
                        "(ProductName, CompanyName, CategoryName, QuantityPerUnit, UnitPrice, UnitsInStock, Discontinued) " +
                        "values(@ProductName, @CompanyName, @CategoryName, @QuantityPerUnit, @UnitPrice, @UnitsInStock, @Discontinued)"

Select SupplierID from Suppliers where CompanyName = 'Testfirma'		/*30*/
Select CategoryID from Categories where CategoryName = 'Testkategorie'	/*9*/

Insert into Categories values('Testkategorie', 'für Tests und co', ' ')
Insert into Suppliers values('Testfirma', 'Max Mustermann', 'CEEEEEEO', 'Musterstraße 1', 'Musterland', NULL, '4122', 'Austria', NULL, NULL, NULL)
select * from Suppliers
select * from Categories

select * from Products order by ProductName desc

select * from vw_simpleProductOverview

update Products
set ProductName = 'Chai',
SupplierID = 1,
CategoryID = 1,
QuantityPerUnit = 'infinite', 
UnitPrice = 20.00,
UnitsInStock = '85',
Discontinued = 0
where ProductID = 1


select * from Employees
select * from EmployeeTerritories
select * from Territories


select top 1 ProductID from vw_simpleProductOverview order by ProductID desc

delete from Products where ProductID >= 78;

delete from vw_simpleProductOverview 

DBCC CHECKIDENT ( Products, RESEED, 77 )		/*To reset the ID when it's too far away*/

SELECT MAX(ProductID) FROM Products

create view vw_simpleCompanyOverview
as
	Select SupplierID as CompanyID , CompanyName, ContactName, ContactTitle, [Address], City, PostalCode, Country, Phone from Suppliers
 
create trigger trig_insertSimpleCompanyOverview
on vw_simpleCompanyOverview
instead of insert
as
begin
set nocount on;
	Insert into Suppliers
	(CompanyName,
	ContactName,
	ContactTitle, 
	[Address], 
	City, 
	Region, 
	PostalCode, 
	Country, 
	Phone, 
	Fax, 
	HomePage) 
	Select 
	CompanyName, 
	ContactName, 
	ContactTitle, 
	[Address], 
	City, 
	null, 
	PostalCode, 
	Country, 
	Phone, 
	null, 
	null from inserted
end

create trigger trig_deleteSimpleCompanyOverview
on vw_simpleCompanyOverview
instead of delete
as
begin
	set nocount on;
	update Products
	set SupplierID = null where SupplierID in (Select CompanyID from deleted) 	
	delete from Suppliers where SupplierID in (Select CompanyID from deleted)
end

create trigger trig_updateSimpleCompanyOverview
on vw_simpleCompanyOverview
instead of update
as
begin
	set nocount on
	update Suppliers
	set CompanyName = (Select CompanyName from inserted),
	ContactName = (Select ContactName from inserted),
	ContactTitle = (Select ContactTitle from inserted),
	[Address] = (Select [Address] from inserted),
	City = (Select City from inserted),
	PostalCode = (Select PostalCode from inserted),
	Country = (Select Country from inserted),
	Phone = (Select Phone from inserted)
	where SupplierID = (Select CompanyID from deleted)
end

update Products set SupplierID = 0 where SupplierID = 

delete from vw_simpleCompanyOverview where CompanyID = 33

Insert into vw_simpleCompanyOverview (CompanyName, ContactName, ContactTitle, [Address], City, PostalCode, Country, Phone) 
values ('NewestCompany', 'Nora Saki', 'CPO', 'Mustergasse 2', 'Linz', '4841', 'Austria', '0664 3833358')
select CompanyID from vw_simpleCompanyOverview

update vw_simpleCompanyOverview set CompanyName = 'much newer Company', ContactName = '', ContactTitle = '', [Address] = '', City = '', PostalCode = '', Country = '', Phone = '' where CompanyID = 33

select * from vw_simpleProductOverview
select * from vw_simpleCompanyOverview
select * from Suppliers
select * from Products
DBCC CHECKIDENT ( Suppliers, RESEED, 29 )

/*P1*/
Select count(ProductID), CompanyName from vw_simpleProductOverview where CompanyName is not null group by CompanyName

/*P2*/
Select count(ProductID), CategoryName from vw_simpleProductOverview where CategoryName is not null group by CategoryName

/*P3*/
Select * from vw_simpleProductOverview where CategoryName = 'Confections'

/*P4*/
Select * from vw_simpleProductOverview where CompanyName = 'Mayumi''s'
Select * from vw_simpleProductOverview where CategoryName = 'Mayumi''s'

/*P5*/
Select * from vw_simpleProductOverview where UnitPrice > 17 and UnitPrice < 22

/*C1*/
Select count(CompanyID), Country from vw_simpleCompanyOverview where Country is not null group by Country

/*C2*/
select * from tfn_lookUpCountry('UK')

/*C3*/
Select *, len(CompanyName) as Namelenght from vw_simpleCompanyOverview order by Namelenght 

/*C4*/
Select * from vw_simpleCompanyOverview where CompanyName like '%_b%'


create Function tfn_lookUpCountry ( @country varchar(20))
returns @returnTable Table
(
	CompanyID int, 
	CompanyName varchar(50), 
	ContactName varchar(40), 
	ContactTitle varchar(30), 
	[Address] varchar(60), 
	City varchar(40), 
	PostalCode varchar(20), 
	Country varchar(20), 
	Phone varchar(20)
)
as
begin
	declare @int int = 10
	set @int = 20
	Insert into @returnTable select * from vw_simpleCompanyOverview where Country = @country
	return 
end


print 'Hello world'
select 'Hello world'

select * from tfn_lookUpCountry('UK')