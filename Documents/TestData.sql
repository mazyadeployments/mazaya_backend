USE [offers.dev]
GO
DELETE
FROM Offer
GO
DELETE
FROM [Collection]
GO
DELETE
FROM Category
GO
DELETE FROM [dbo].[OfferDocument]
GO
DELETE FROM [dbo].[Document]
GO

SET IDENTITY_INSERT Banner OFF
SET IDENTITY_INSERT Company OFF
SET IDENTITY_INSERT Country OFF
SET IDENTITY_INSERT Help OFF
SET IDENTITY_INSERT NotificationType OFF
SET IDENTITY_INSERT MailStorage OFF
SET IDENTITY_INSERT [User] OFF
SET IDENTITY_INSERT UserNotification OFF
SET IDENTITY_INSERT UserProfile OFF
SET IDENTITY_INSERT Collection OFF
SET IDENTITY_INSERT Category OFF
SET IDENTITY_INSERT Offer OFF
SET IDENTITY_INSERT OfferDocument OFF
GO
DBCC CHECKIDENT ('Offer', RESEED, 1)
DBCC CHECKIDENT ('Collection', RESEED, 1)
DBCC CHECKIDENT ('Category', RESEED, 1)
GO

SET IDENTITY_INSERT [dbo].[Category] ON 
GO
INSERT [dbo].[Category] ([Id], [Title], [Description], [ImageUrl], [ImageUrlCrop], [CreatedOn], [CreatedBy], [UpdatedOn], [UpdatedBy]) VALUES (1, N'Hotel & Resort', N'Description of Hotel & Resort ', N'imageurl link', N'imageurl crop link', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), N'predrag', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), N'pedja')
GO
INSERT [dbo].[Category] ([Id], [Title], [Description], [ImageUrl], [ImageUrlCrop], [CreatedOn], [CreatedBy], [UpdatedOn], [UpdatedBy]) VALUES (2, N'Food & Beverages', N'Description of Food & Beverages ', N'imageurl link 2', N'imageurl crop link 2', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), N'predrag', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), N'pedja')
GO
INSERT [dbo].[Category] ([Id], [Title], [Description], [ImageUrl], [ImageUrlCrop], [CreatedOn], [CreatedBy], [UpdatedOn], [UpdatedBy]) VALUES (3, N'Entertainment', N'Description of Entertainment ', N'imageurl link 3', N'imageurl crop link 3', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), N'predrag', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), N'pedja')
GO
INSERT [dbo].[Category] ([Id], [Title], [Description], [ImageUrl], [ImageUrlCrop], [CreatedOn], [CreatedBy], [UpdatedOn], [UpdatedBy]) VALUES (4, N'Medical', N'Description of medical ', N'imageurl link 4', N'imageurl crop link 4', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), N'predrag', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), N'pedja')
GO
INSERT [dbo].[Category] ([Id], [Title], [Description], [ImageUrl], [ImageUrlCrop], [CreatedOn], [CreatedBy], [UpdatedOn], [UpdatedBy]) VALUES (5, N'Shopping', N'Description of Shopping ', N'imageurl link 5', N'imageurl crop link 5', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), N'predrag', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), N'pedja')
GO
INSERT [dbo].[Category] ([Id], [Title], [Description], [ImageUrl], [ImageUrlCrop], [CreatedOn], [CreatedBy], [UpdatedOn], [UpdatedBy]) VALUES (6, N'Automotive', N'Description of Automotive ', N'imageurl link 6', N'imageurl crop link 6', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), N'predrag', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), N'pedja')
GO
INSERT [dbo].[Category] ([Id], [Title], [Description], [ImageUrl], [ImageUrlCrop], [CreatedOn], [CreatedBy], [UpdatedOn], [UpdatedBy]) VALUES (7, N'Travel', N'Description of Travel ', N'imageurl link 7', N'imageurl crop link 7', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), N'predrag', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), N'pedja')
GO
INSERT [dbo].[Category] ([Id], [Title], [Description], [ImageUrl], [ImageUrlCrop], [CreatedOn], [CreatedBy], [UpdatedOn], [UpdatedBy]) VALUES (8, N'Beauty & Spa', N'Description of Beauty & Spa ', N'imageurl link 8', N'imageurl crop link 8', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), N'predrag', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), N'pedja')
GO
INSERT [dbo].[Category] ([Id], [Title], [Description], [ImageUrl], [ImageUrlCrop], [CreatedOn], [CreatedBy], [UpdatedOn], [UpdatedBy]) VALUES (9, N'Flower & Chocolate', N'Description of Flower & Chocolate ', N'imageurl link 9', N'imageurl crop link 9', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), N'predrag', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), N'pedja')
GO
INSERT [dbo].[Category] ([Id], [Title], [Description], [ImageUrl], [ImageUrlCrop], [CreatedOn], [CreatedBy], [UpdatedOn], [UpdatedBy]) VALUES (10, N'Home Service', N'Description of Home Service ', N'imageurl link 10', N'imageurl crop link 10', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), N'predrag', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), N'pedja')
GO
INSERT [dbo].[Category] ([Id], [Title], [Description], [ImageUrl], [ImageUrlCrop], [CreatedOn], [CreatedBy], [UpdatedOn], [UpdatedBy]) VALUES (11, N'Home & Furniture', N'Description of Home & Furniture ', N'imageurl link 11', N'imageurl crop link 11', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), N'predrag', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), N'pedja')
GO
INSERT [dbo].[Category] ([Id], [Title], [Description], [ImageUrl], [ImageUrlCrop], [CreatedOn], [CreatedBy], [UpdatedOn], [UpdatedBy]) VALUES (12, N'Home & Fitness', N'Description of Home & Fitness ', N'imageurl link 12', N'imageurl crop link 12', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), N'predrag', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), N'pedja')
GO
INSERT [dbo].[Category] ([Id], [Title], [Description], [ImageUrl], [ImageUrlCrop], [CreatedOn], [CreatedBy], [UpdatedOn], [UpdatedBy]) VALUES (13, N'Education', N'Description of Education ', N'imageurl link 13', N'imageurl crop link 13', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), N'predrag', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), N'pedja')
GO
INSERT [dbo].[Category] ([Id], [Title], [Description], [ImageUrl], [ImageUrlCrop], [CreatedOn], [CreatedBy], [UpdatedOn], [UpdatedBy]) VALUES (14, N'Adventure & Sports', N'Description of Adventure & Sports ', N'imageurl link 14', N'imageurl crop link 14', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), N'predrag', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), N'pedja')
GO
INSERT [dbo].[Category] ([Id], [Title], [Description], [ImageUrl], [ImageUrlCrop], [CreatedOn], [CreatedBy], [UpdatedOn], [UpdatedBy]) VALUES (15, N'Properties', N'Description of Properties ', N'imageurl link 15', N'imageurl crop link 15', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), N'predrag', CAST(N'0001-01-01T00:00:00.0000000' AS DateTime2), N'pedja')
GO
SET IDENTITY_INSERT [dbo].[Category] OFF
GO

SET IDENTITY_INSERT [dbo].[Collection] ON 
GO
INSERT [dbo].[Collection] ([Id], [Title], [Description], [ValidUntil], [Location], [SponsorLogoImageUrl], [SponsorLogoImageUrlCrop], [ImageUrl], [ImageUrlCrop], [CreatedOn], [CreatedBy], [UpdatedOn], [UpdatedBy]) VALUES (1, N'a', N'a', CAST(N'2019-01-01T00:00:00.0000000' AS DateTime2), N'1', N'1', N'1', N'1', N'1', CAST(N'2019-01-01T00:00:00.0000000' AS DateTime2), N'a', CAST(N'2019-01-01T00:00:00.0000000' AS DateTime2), N'a')
GO
SET IDENTITY_INSERT [dbo].[Collection] OFF
GO

USE [offers.dev]
GO
SET IDENTITY_INSERT [dbo].[Offer] ON 
GO
INSERT [dbo].[Offer] ([Id], [Title], [Description], [CategoryId], [CollectionId], [Tag], [PromotionCode], [Address], [City], [Country], [Price], [DiscountPercentage], [ValidFrom], [ValidUntil], [WhatYouGet], [PriceList], [TermsAndCondition], [AboutCompany], [Longtitude], [Latitude], [CreatedOn], [CreatedBy], [UpdatedOn], [UpdatedBy], [FlagIsLatest]) VALUES (8, N'Test Offer 8', N'Test Offer 8 long description ', 1, 1, N'1', N'1', N'Test Address', N'Abu Dhabi', N'UAE', CAST(800.000000000000 AS Decimal(28, 12)), CAST(10.000000000000 AS Decimal(28, 12)), CAST(N'2020-01-01T00:00:00.0000000' AS DateTime2), CAST(N'2020-12-31T00:00:00.0000000' AS DateTime2), N'All what you see in menu', N'Small price list', N'Everithing included no terms', N'Large enterprise company', NULL, NULL, CAST(N'2020-01-01T00:00:00.0000000' AS DateTime2), N'admin', CAST(N'2020-01-01T00:00:00.0000000' AS DateTime2), N'admin', 1)
GO
SET IDENTITY_INSERT [dbo].[Offer] OFF
GO

UPDATE Offer
SET Title = 'Test Offer ' + cast(id AS VARCHAR(10)),
	Description = 'Test Offer ' + cast(id AS VARCHAR(10)) + ' long description ',
	Price = Id * 10

UPDATE Offer
	SET CreatedOn = DATEADD(DAY, 10, GETDATE()),
	UpdatedOn = DATEADD(DAY, 1, GETDATE())

GO

INSERT [dbo].[Document] ([Id], [Name], [MimeType], [StorageType], [Size], [Content], [StoragePath], [CreatedOn], [CreatedBy], [UpdatedOn], [UpdatedBy], [ParentId]) VALUES (N'07fcdce4-0e91-4e3f-9bbd-29cbb6439fe0', N'slika.jpg', N'image/jpeg', N'azurestorage', 99999, NULL, N'slika.jpg', CAST(N'2020-01-01T00:00:00.0000000' AS DateTime2), N'admin', CAST(N'2020-01-01T00:00:00.0000000' AS DateTime2), N'admin', NULL)
GO
SET IDENTITY_INSERT [dbo].[OfferDocument] ON 
GO
INSERT [dbo].[OfferDocument] ([Id], [OfferId], [DocumentId], [CreatedOn], [CreatedBy], [UpdatedOn], [UpdatedBy], [Type]) VALUES (3, 8, N'07fcdce4-0e91-4e3f-9bbd-29cbb6439fe0', CAST(N'2020-01-01T00:00:00.0000000' AS DateTime2), N'admin', CAST(N'2020-01-01T00:00:00.0000000' AS DateTime2), N'admin', 6)
GO



  INSERT INTO [dbo].[OfferCategory] (OfferId, CategoryId)
  VALUES (2,1),
		 (2,2),
		 (2,3),
		 (3,4),
		 (3,5),
		 (3,6),
		 (4,7),
		 (5,8),
		 (5,9),
		 (6,10),
		 (6,11),
		 (7,12),
		 (7,13),
		 (8,14),
		 (8,15),
		 (8,16),
		 (9,17),
		 (9,18),
		 (10,19),
		 (10,20),
		 (10,21),
		 (11,22),
		 (11,1),
		 (11,2),
		 (12,3),
		 (13,4),
		 (13,5),
		 (14,6),
		 (14,7),
		 (15,8),
		 (15,9),
		 (16,10),
		 (16,11),
		 (17,12),
		 (18,13),
		 (19,14),
		 (20,15),
		 (20,16),
		 (21,17),
		 (22,18),
		 (22,19),
		 (23,20),
		 (23,21)
 
 
   INSERT INTO [dbo].[OfferCollection] (OfferId, CollectionId)
  VALUES (2,1),
		 (2,2),
		 (2,3),
		 (3,4),
		 (3,5),
		 (3,6),
		 (4,7),
		 (5,8),
		 (5,9),
		 (6,10),
		 (6,11),
		 (7,12),
		 (7,13),
		 (8,14),
		 (8,15),
		 (8,16),
		 (9,17),
		 (9,18),
		 (10,19),
		 (10,20),
		 (10,21),
		 (11,22),
		 (11,1),
		 (11,2),
		 (12,3),
		 (13,4),
		 (13,5),
		 (14,6),
		 (14,7),
		 (15,8),
		 (15,9),
		 (16,10),
		 (16,11),
		 (17,12),
		 (18,13),
		 (19,14),
		 (20,15),
		 (20,16),
		 (21,17),
		 (22,18),
		 (22,19),
		 (23,20),
		 (23,21)
  

INSERT INTO [dbo].[OfferTag] (OfferId, TagId)
  VALUES (2,1),
		 (2,2),
		 (2,3),
		 (3,4),
		 (3,5),
		 (3,6),
		 (4,7),
		 (5,8),
		 (5,9),
		 (6,10),
		 (6,11),
		 (7,1),
		 (7,2),
		 (8,3),
		 (8,4),
		 (8,5),
		 (9,6),
		 (9,7),
		 (10,8),
		 (10,9),
		 (10,10),
		 (11,11),
		 (11,1),
		 (11,2),
		 (12,3),
		 (13,4),
		 (13,5),
		 (14,6),
		 (14,7),
		 (15,8),
		 (15,9),
		 (16,10),
		 (16,11),
		 (17,1),
		 (18,2),
		 (19,3),
		 (20,4),
		 (20,5),
		 (21,6),
		 (22,7),
		 (22,8),
		 (23,9),
		 (23,10)
  

insert into Company ([Name],[CreatedOn]
      ,[CreatedBy]
      ,[UpdatedOn]
      ,[UpdatedBy]
      ,[SupplierId])

values('Abu Dhabi National Hotels', '2020-01-01 00:00:00.0000000', 'B6E82FB2-CAC5-43E4-BCF4-4156AA829D78','2020-01-01 00:00:00.0000000', 'B6E82FB2-CAC5-43E4-BCF4-4156AA829D78', '65584013-5D0E-4807-AB08-7B95F1B09685')



  insert into CompanyLocation ([Longtitude]
      ,[Latitude]
      ,[Address]
      ,[Vicinity]
      ,[Country]
      ,[CompanyId]
      ,[CreatedBy]
      ,[CreatedOn]
      ,[UpdatedBy]
      ,[UpdatedOn])

values('54.394712', '24.455136', 'Dusit Thani Abu Dhabi', 'Abu Dhabi', 'United Arab Emirates', 5, 'B6E82FB2-CAC5-43E4-BCF4-4156AA829D78','2020-01-01 00:00:00.0000000','B6E82FB2-CAC5-43E4-BCF4-4156AA829D78','2020-01-01 00:00:00.0000000'),
      ('54.437418', '24.429709', 'Holiday Inn Abu Dhabi', 'Abu Dhabi', 'United Arab Emirates', 5, 'B6E82FB2-CAC5-43E4-BCF4-4156AA829D78','2020-01-01 00:00:00.0000000','B6E82FB2-CAC5-43E4-BCF4-4156AA829D78','2020-01-01 00:00:00.0000000'),
	  ('54.473146', '24.424443', 'Park Rotana', 'Abu Dhabi', 'United Arab Emirates', 5, 'B6E82FB2-CAC5-43E4-BCF4-4156AA829D78','2020-01-01 00:00:00.0000000','B6E82FB2-CAC5-43E4-BCF4-4156AA829D78','2020-01-01 00:00:00.0000000')

DECLARE @cnt INT = 0;

WHILE @cnt < 4
BEGIN
   
	INSERT INTO Offer
	SELECT 
		[Title],
		[Description],
		[CategoryId],
		[CollectionId],
		[Tag],
		[PromotionCode],
		[Address],
		[City],
		[Country],
		[Price],
		[DiscountPercentage],
		[ValidFrom],
		[ValidUntil],
		[WhatYouGet],
		[PriceList],
		[TermsAndCondition],
		[AboutCompany],
		[Longtitude],
		[Latitude],
		[CreatedOn],
		[CreatedBy],
		[UpdatedOn],
		[UpdatedBy],
		[FlagIsLatest],
		[BannerId]
	FROM Offer

   SET @cnt = @cnt + 1;
END;

GO

GO