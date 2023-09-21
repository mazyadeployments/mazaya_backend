-- OVAKO TREBA DA IZGLEDA POZIV FUNKCIJE, SAMO IZMENITI PARAMETRE DA ODGOVARAJU ONIMA IZ BAZE
-- EXEC PopulateRoadshowsWithDummyData @CompanyID = , @TagID = , @CategoryID = , @CollectionID = , @AdminID = , @SupplierID = , @UserID = ;

CREATE  PROCEDURE PopulateRoadshowsWithDummyData
(
	@CompanyID INT, @TagID INT, @CategoryID INT, @CollectionID INT, @AdminID NVARCHAR(50), @SupplierID NVARCHAR(50), @UserID NVARCHAR(50)
)
AS
BEGIN
	BEGIN TRANSACTION [RoadshowTransaction]

	-- Default Location 1.
	DECLARE @DefaultLocationLongitude DECIMAL = 54.3293559551;
	DECLARE @DefaultLocationLatitude DECIMAL = 24.4560175336;
	DECLARE @DefaultLocationAddress NVARCHAR(50) = '5 Zayed The First Street';
	DECLARE @DefaultLocationVicinity NVARCHAR(50) = 'Abu Dhabi';
	DECLARE @DefaultLocationCountry NVARCHAR(50) = 'United Arab Emirates';

	INSERT INTO DefaultLocation
	(
		Longitude, Latitude, [Address], Vicinity, Country
	) 
	VALUES
	(
		@DefaultLocationLongitude, @DefaultLocationLatitude, @DefaultLocationAddress, @DefaultLocationVicinity, @DefaultLocationCountry
	)
	DECLARE @RoadshowLocationDefaultLocationId INT = IDENT_CURRENT('DefaultLocation');

	-- Roadshow 2.
	DECLARE @RoadshowStatus INT = 3;
	DECLARE @RoadshowTitle NVARCHAR(50) = 'Roadshow Test';
	DECLARE @RoadshowDateFrom NVARCHAR(50) = GETDATE();
	DECLARE @RoadshowDateTo NVARCHAR(50) = GETDATE();
	DECLARE @RoadshowDescription NVARCHAR(50) = 'Roadshow Description';
	DECLARE @RoadshowComment NVARCHAR(50) = 'Roadshow Comment';
	DECLARE @RoadshowCreatedOn NVARCHAR(50) = GETDATE();
	DECLARE @RoadshowCreatedBy NVARCHAR(50) = @AdminID;
	DECLARE @RoadshowUpdatedOn NVARCHAR(50) = GETDATE();
	DECLARE @RoadshowUpdatedBy NVARCHAR(50) = @AdminID;

	INSERT INTO Roadshow
	(
		[Status], Title, DateFrom, DateTo, [Description], Comment, CreatedOn, CreatedBy, UpdatedOn, UpdatedBy
	) 
	VALUES
	(
		@RoadshowStatus, @RoadshowTitle, @RoadshowDateFrom, @RoadshowDateTo, @RoadshowDescription, @RoadshowComment, @RoadshowCreatedOn, @RoadshowCreatedBy, @RoadshowUpdatedOn, @RoadshowUpdatedBy
	)
	DECLARE @RoadshowID INT = IDENT_CURRENT('Roadshow');


	-- Document (For Roadshow)
	DECLARE @DocumentID UNIQUEIDENTIFIER = NEWID();
	DECLARE @DocumentName NVARCHAR(50) = '20-_off-crop--38ddbd79-a21f-463f-80f2-e9a3cba7810b.jpg';
	DECLARE @DocumentMimeType NVARCHAR(50) = 'image/jpeg';
	DECLARE @DocumentStorageType NVARCHAR(50) = 'azureblobstorage';
	DECLARE @DocumentSize NVARCHAR(50) = '126967';
	DECLARE @DocumentContent VARBINARY = CONVERT(VARBINARY, '0x047CF6791BE2A4A778CFFCE7FB8FF27F');
	DECLARE @DocumentStoragePath NVARCHAR(50) = 'documents/13a0a19c-865f-40d0-a2a2-00728664c327';
	DECLARE @DocumentCreatedOn NVARCHAR(50) = GETDATE();
	DECLARE @DocumentCreatedBy NVARCHAR(50) = @AdminID;
	DECLARE @DocumentUpdatedOn NVARCHAR(50) = GETDATE();
	DECLARE @DocumentUpdatedBy NVARCHAR(50) = @AdminID;
	DECLARE @DocumentParentID NVARCHAR(50) = NULL;
	INSERT INTO Document
	(
		Id, [Name], MimeType, StorageType, Size, Content, StoragePath, CreatedOn, CreatedBy, UpdatedOn, UpdatedBy, ParentId
	)
	VALUES
	(	
		@DocumentID, @DocumentName, @DocumentMimeType, @DocumentStorageType, @DocumentSize, @DocumentContent, @DocumentStoragePath, @DocumentCreatedOn, @DocumentCreatedBy, @DocumentUpdatedOn, @DocumentUpdatedBy, @DocumentParentId
	)
	DECLARE @DocumentForRoadshowID UNIQUEIDENTIFIER = @DocumentID;


	-- Roadshow Document 3.
	DECLARE @RoadshowDocumentType INT = 2;
	INSERT INTO RoadshowDocument
	(
		RoadshowId, DocumentId, CreatedOn, CreatedBy, UpdatedOn, UpdatedBy, [Type]
	)
	VALUES
	(
		@RoadshowID, @DocumentForRoadshowID, @DocumentCreatedOn, @DocumentCreatedBy, @DocumentUpdatedOn, @DocumentUpdatedBy, @RoadshowDocumentType
	)
	DECLARE @RoadshowDocumentID INT = IDENT_CURRENT('RoadshowDocument');


	-- RoadshowLocation 4.
	INSERT INTO RoadshowLocation
	(
		RoadshowId, DefaultLocationId
	)
	VALUES
	(
		@RoadshowID, @RoadshowLocationDefaultLocationId
	)
	DECLARE @RoadshowLocationID INT = IDENT_CURRENT('RoadshowLocation');


	-- RoadshowInvite 5.
	DECLARE @RoadshowInviteStatus INT = 3;
	DECLARE @RoadshowInviteCreatedOn NVARCHAR(50) = GETDATE();
	DECLARE @RoadshowInviteCreatedBy NVARCHAR(50) = @AdminID;
	DECLARE @RoadshowInviteUpdatedOn NVARCHAR(50) = GETDATE();
	DECLARE @RoadshowInviteUpdatedBy NVARCHAR(50) = @AdminID;

	INSERT INTO RoadshowInvite
	(
		CompanyId, [Status], CreatedOn, CreatedBy, UpdatedOn, UpdatedBy, RoadshowId
	)
	VALUES
	(
		@CompanyID, @RoadshowInviteStatus, @RoadshowInviteCreatedOn, @RoadshowInviteCreatedBy, @RoadshowInviteUpdatedOn, @RoadshowInviteUpdatedBy, @RoadshowID
	)
	DECLARE @RoadshowInviteID INT = IDENT_CURRENT('RoadshowInvite');


	-- RoadshowComment 6.
	DECLARE @RoadshowCommentText NVARCHAR(50) = 'Comment text';
	DECLARE @RoadshowCommentCreatedOn NVARCHAR(50) = GETDATE();
	DECLARE @RoadshowCommentCreatedBy NVARCHAR(50) = @SupplierID;

	INSERT INTO RoadshowComment
	(
		[Text], CreatedOn, CreatedBy, RoadshowId, RoadshowInviteId
	)
	VALUES
	(
		@RoadshowCommentText, @RoadshowCommentCreatedOn, @RoadshowCommentCreatedBy, @RoadshowID, @RoadshowInviteID
	)
	DECLARE @RoadshowCommentID INT = IDENT_CURRENT('RoadshowComment');


	-- Roadshow Event 7.
	DECLARE @RoadshowEventDateFrom NVARCHAR(50) = GETDATE();
	DECLARE @RoadshowEventDateTo NVARCHAR(50) = GETDATE();
	INSERT INTO RoadshowEvent
	(
		DateFrom, DateTo, RoadshowInviteId, RoadshowLocationDefaultLocationId, RoadshowLocationRoadshowId
	)
	VALUES
	(
		@RoadshowEventDateFrom, @RoadshowEventDateTo, @RoadshowInviteID, @RoadshowLocationDefaultLocationId, @RoadshowID
	)
	DECLARE @RoadshowEventID INT = IDENT_CURRENT('RoadshowEvent');


	-- RoadshowProposal 8.
	DECLARE @RoadshowProposalStatus INT = 3;
	DECLARE @RoadshowProposalRoadshowDetails NVARCHAR(50) = 'Roadshow Proposal Roadshow Details';
	DECLARE @RoadshowProposalEquipmentItem NVARCHAR(50) = 'Roadshow Proposal Equipment Item';
	DECLARE @RoadshowProposalSubject NVARCHAR(50) = 'Roadshow Proposal Subjecy';
	DECLARE @RoadshowProposalTermsAndCondition NVARCHAR(50) = 'Roadshow Proposal Terms And Condition';
	DECLARE @RoadshowProposalTermsAndConditionChecked BIT = 1;
	DECLARE @RoadshowProposalOfferEffectiveDate NVARCHAR(50) = GETDATE();
	DECLARE @RoadshowProposalExpiryDate NVARCHAR(50) = GETDATE();
	DECLARE @RoadshowProposalName NVARCHAR (50) = 'Proposal Name';
	DECLARE @RoadshowProposalTitle NVARCHAR(50) = 'Proposal Title';
	DECLARE @RoadshowProposalSignature NVARCHAR(50) = 'Proposal Signature';
	DECLARE @RoadshowProposalManager NVARCHAR(50) = 'Proposal Manager';
	DECLARE @RoadshowProposalCreatedOn NVARCHAR(50) = GETDATE();
	DECLARE @RoadshowProposalCreatedBy NVARCHAR(50) = @SupplierID;
	DECLARE @RoadshowProposalUpdatedOn NVARCHAR(50) = GETDATE();
	DECLARE @RoadshowProposalUpdatedBy NVARCHAR(50) = @SupplierID;

	INSERT INTO RoadshowProposal
	(
		[Status], RoadshowDetails, EquipmentItem, [Subject], CompanyId, TermsAndCondition, TermsAndConditionChecked, 
		OfferEffectiveDate, ExpiryDate, [Name], Title, [Signature], Manager, CreatedOn, CreatedBy, UpdatedOn, UpdatedBy
	)
	VALUES
	(
		@RoadshowProposalStatus, @RoadshowProposalRoadshowDetails, @RoadshowProposalEquipmentItem, @RoadshowProposalSubject, @CompanyID, @RoadshowProposalTermsAndCondition, @RoadshowProposalTermsAndConditionChecked,
		@RoadshowProposalOfferEffectiveDate, @RoadshowProposalExpiryDate, @RoadshowProposalName, @RoadshowProposalTitle, @RoadshowProposalSignature, @RoadshowProposalManager, @RoadshowProposalCreatedOn,
		@RoadshowProposalCreatedBy, @RoadshowProposalUpdatedOn, @RoadshowProposalUpdatedBy
	)
	DECLARE @RoadshowProposalID INT = IDENT_CURRENT('RoadshowProposal');


	-- Roadshow Offer 9.
	DECLARE @RoadshowOfferTitle NVARCHAR(50) = 'Offer Title';
	DECLARE @RoadshowOfferRoadshowDetails NVARCHAR(50) = 'Offer Details';
	DECLARE @RoadshowOfferEquipmentItem NVARCHAR(50) = 'Offer Equipment Item';
	DECLARE @RoadshowOfferPromotionCode NVARCHAR(50) = 'Offer Promotion Code';
	DECLARE @RoadshowOfferStatus INT = 3;
	DECLARE @RoadshowOfferCreatedOn NVARCHAR(50) = GETDATE();
	DECLARE @RoadshowOfferCreatedBy NVARCHAR(50) = @SupplierID;
	DECLARE @RoadshowOfferUpdatedOn NVARCHAR(50) = GETDATE();
	DECLARE @RoadshowOfferUpdatedBy NVARCHAR(50) = @SupplierID;

	INSERT INTO RoadshowOffer
	(
		Title, RoadshowProposalId, RoadshowDetails, EquipmentItem, PromotionCode, [Status], CreatedOn, CreatedBy, UpdatedOn, UpdatedBy, RoadshowEventId
	)
	VALUES
	(
		@RoadshowOfferTitle, @RoadshowProposalID, @RoadshowOfferRoadshowDetails, @RoadshowOfferEquipmentItem, @RoadshowOfferPromotionCode, @RoadshowOfferStatus,
		@RoadshowOfferCreatedOn, @RoadshowOfferCreatedBy, @RoadshowOfferUpdatedOn, @RoadshowOfferUpdatedBy, @RoadshowEventID
	)
	DECLARE @RoadshowOfferID INT = IDENT_CURRENT('RoadshowOffer');


	-- RoadshowVoucher 10.
	DECLARE @RoadshowVoucherQuantity INT = 10;
	DECLARE @RoadshowVoucherDetails NVARCHAR(50) = 'Details';
	DECLARE @RoadshowVoucherValidity NVARCHAR(50) = GETDATE();

	INSERT INTO RoadshowVoucher
	(
		Quantity, Details, RoadshowOfferId, RoadshowProposalId, Validity
	)
	VALUES
	(
		@RoadshowVoucherQuantity, @RoadshowVoucherDetails, @RoadshowOfferID, @RoadshowProposalID, @RoadshowVoucherValidity
	)
	DECLARE @RoadshowVoucherID INT = IDENT_CURRENT('RoadshowVoucher');


	-- RoadshowOfferRating 11.
	DECLARE @RoadshowOfferRatingRating INT = 5;
	DECLARE @RoadshowOfferRatingCommentText NVARCHAR(50) = 'Excellent';
	DECLARE @RoadshowOfferRatingStatus NVARCHAR(50) = 'Public';
	DECLARE @RoadshowOfferRatingCreatedOn NVARCHAR(50) = GETDATE();
	DECLARE @RoadshowOfferRatingCreatedBy NVARCHAR(50) = @UserID;
	DECLARE @RoadshowOfferRatingUpdatedOn NVARCHAR(50) = GETDATE();
	DECLARE @RoadshowOfferRatingUpdatedBy NVARCHAR(50) = @UserID;

	INSERT INTO RoadshowOfferRating
	(
		RoadshowOfferId, ApplicationUserId, Rating, CommentText, [Status], CreatedOn, CreatedBy, UpdatedOn, UpdatedBy
	)
	VALUES
	(
		@RoadshowOfferID, @UserID, @RoadshowOfferRatingRating, @RoadshowOfferRatingCommentText, @RoadshowOfferRatingStatus, 
		@RoadshowOfferRatingCreatedOn,@RoadshowOfferRatingCreatedBy, @RoadshowOfferRatingUpdatedOn, @RoadshowOfferRatingUpdatedBy
	)
	DECLARE @RoadshowOfferRatingID INT = IDENT_CURRENT('RoadshowOfferRating');


	-- RoadshowOfferTag 12.
	INSERT INTO RoadshowOfferTag
	(
		RoadshowOfferId, TagId
	)
	VALUES
	(
		@RoadshowOfferID, @TagID
	)
	DECLARE @RoadshowOfferTagID INT = IDENT_CURRENT('RoadshowOfferTag');



	-- Document (For RoadshowOfferProposal) 13.
	SET @DocumentID = NEWID();

	INSERT INTO Document
	(
		Id, [Name], MimeType, StorageType, Size, Content, StoragePath, CreatedOn, CreatedBy, UpdatedOn, UpdatedBy, ParentId
	)
	VALUES
	(	
		@DocumentID, @DocumentName, @DocumentMimeType, @DocumentStorageType, @DocumentSize, @DocumentContent, @DocumentStoragePath, @DocumentCreatedOn, @DocumentCreatedBy, @DocumentUpdatedOn, @DocumentUpdatedBy, @DocumentParentId
	)
	DECLARE @DocumentForRoadshowOfferProposalID UNIQUEIDENTIFIER = @DocumentID;--IDENT_CURRENT('Document');


	DECLARE @RoadshowOfferProposalDocumentType NVARCHAR(50) = 2;

	INSERT INTO RoadshowOfferProposalDocument
	(
		RoadshowOfferProposalId, DocumentId, CreatedOn, CreatedBy, UpdatedOn, UpdatedBy, [Type], RoadshowProposalId
	)
	VALUES
	(
		@RoadshowOfferID, @DocumentForRoadshowOfferProposalID, @DocumentCreatedOn, @DocumentCreatedBy, @DocumentUpdatedOn, @DocumentUpdatedBy, @RoadshowOfferProposalDocumentType, @RoadshowProposalID
	)
	DECLARE @RoadshowOfferProposalDocumentID INT = IDENT_CURRENT('RoadshowOfferDocument');




	-- Document (For RoadshowOffer) 14.
	SET @DocumentID = NEWID();
	INSERT INTO Document
	(
		Id, [Name], MimeType, StorageType, Size, Content, StoragePath, CreatedOn, CreatedBy, UpdatedOn, UpdatedBy, ParentId
	)
	VALUES
	(	
		@DocumentID, @DocumentName, @DocumentMimeType, @DocumentStorageType, @DocumentSize, @DocumentContent, @DocumentStoragePath, @DocumentCreatedOn, @DocumentCreatedBy, @DocumentUpdatedOn, @DocumentUpdatedBy, @DocumentParentId
	)
	DECLARE @DocumentForRoadshowOfferID UNIQUEIDENTIFIER = @DocumentID;--IDENT_CURRENT('Document');

	-- RoadshowOffer Document
	DECLARE @RoadshowOfferDocumentType NVARCHAR(50) = 2;
	DECLARE @OriginalImageID NVARCHAR(50) = @DocumentForRoadshowOfferID;
	DECLARE @X1 INT = 0;
	DECLARE @X2 INT = 0;
	DECLARE @Y1 INT = 0;
	DECLARE @Y2 INT = 0;
	DECLARE @CropX1 INT = 0;
	DECLARE @CropX2 INT = 0;
	DECLARE @CropY1 INT = 0;
	DECLARE @CropY2 INT = 0;

	INSERT INTO RoadshowOfferDocument
	(
		RoadshowOfferId, DocumentId, CreatedOn, CreatedBy, UpdatedOn, UpdatedBy, [Type], OriginalImageId, X1, Y1, X2, Y2, cropX1, cropX2, cropY1, cropY2
	)
	VALUES
	(
		@RoadshowOfferID, @DocumentForRoadshowOfferID, @DocumentCreatedOn, @DocumentCreatedBy, @DocumentUpdatedOn, @DocumentUpdatedBy, @RoadshowDocumentType, 
		@OriginalImageID, @X1, @X2, @Y1, @Y2, @CropX1, @CropX2, @CropY1, @CropY2
	)
	DECLARE @RoadshowOfferDocumentID INT = IDENT_CURRENT('RoadshowOfferDocument');


	-- RoadshowOfferCategory 15.
	INSERT INTO RoadshowOfferCategory
	(
		RoadshowOfferId, CategoryId
	)
	VALUES
	(
		@RoadshowOfferID, @CategoryID
	)
	DECLARE @RoadshowOfferCategoryID INT = IDENT_CURRENT('RoadshowOfferCategory');



	-- RoadshowOfferCollection 16.
	INSERT INTO RoadshowOfferCollection
	(
		RoadshowOfferId, CollectionId
	)
	VALUES
	(
		@RoadshowOfferID, @CollectionID
	)
	DECLARE @RoadshowOfferCollectionID INT = IDENT_CURRENT('RoadshowOfferCollection');


	COMMIT TRANSACTION [RoadshowTransaction]
END