
GO
USE [__Assessment__]
GO

/****** Object:  StoredProcedure [dbo].[SP_GetRootConfig]    Script Date: 8/18/2020 5:27:41 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO


CREATE PROCEDURE [dbo].[SP_GetRootConfig]
AS
DECLARE @Root_Config TABLE (
    Id int,
    PartNo nvarchar(max),
    Conf_Name nvarchar(max),
	IsActive bit

);
insert into @Root_Config
select Id,PartNo,ConfigurationName, IsActive from [2123__637328370661132736__SolidWorksNative].[Configuration]
 where IsRoot=1 and ConfigurationName <> '' order by IsActive desc

 select Id,Conf_Name from @Root_Config order by IsActive desc
GO



USE [__Assessment__]
GO

/****** Object:  StoredProcedure [dbo].[GetPartNodeDetails]    Script Date: 8/18/2020 5:27:26 PM ******/
SET ANSI_NULLS ON
GO

SET QUOTED_IDENTIFIER ON
GO

--GetPartNodeDetails 3,1

CREATE PROCEDURE [dbo].[GetPartNodeDetails]
(
  @PartId INT,
  @ShowPart bit 
  )
  AS  
 
 DECLARE @Root_Config TABLE (
    Id int,
    ParentConfigIndex int,
    ChildConfigIndex int,
	Qty float,
	Excluded bit,
	Suppressed bit

);


  WITH tblChild AS
   ( SELECT * FROM [2123__637328370661132736__SolidWorksNative].Bom  
    WHERE ParentConfigIndex = @PartId
	 UNION ALL     
	 SELECT B.* FROM   [2123__637328370661132736__SolidWorksNative].Bom  B
	 JOIN tblChild  
	 ON   B.ParentConfigIndex = tblChild.ChildConfigIndex ) 

	 INSERT INTO @Root_Config
	 select * From tblChild



  	IF(@ShowPart=1)
	BEGIN

		SELECT (SELECT PartNo  FROM
		[2123__637328370661132736__SolidWorksNative].Configuration
	  	WHERE Id = ParentConfigIndex) ParentName,ParentConfigIndex,
	
		  (SELECT Description  FROM
		[2123__637328370661132736__SolidWorksNative].Configuration
	  	WHERE Id = ParentConfigIndex) Root_Description,
	  
	  (SELECT Description  FROM
		[2123__637328370661132736__SolidWorksNative].Configuration
	  	WHERE Id = ChildConfigIndex) Description,


		(SELECT PartNo  FROM 
		[2123__637328370661132736__SolidWorksNative].Configuration 
	   	WHERE Id = ChildConfigIndex) ChildName,ChildConfigIndex,Qty
		FROM @Root_Config ORDER BY ParentConfigIndex,ChildConfigIndex
		OPTION(MAXRECURSION 0)    
	 END
	 ELSE
	 BEGIN
	    SELECT (SELECT FilePath  FROM
		[2123__637328370661132736__SolidWorksNative].Configuration
	  	WHERE Id = ParentConfigIndex) ParentName,ParentConfigIndex,

	 	  (SELECT Description  FROM
		[2123__637328370661132736__SolidWorksNative].Configuration
	  	WHERE Id = ParentConfigIndex) Root_Description,

	 	  (SELECT Description  FROM
		[2123__637328370661132736__SolidWorksNative].Configuration
	  	WHERE Id = ChildConfigIndex) Description,

		(SELECT FilePath   FROM 
		[2123__637328370661132736__SolidWorksNative].Configuration 
	   	WHERE Id = ChildConfigIndex) ChildName,ChildConfigIndex,Qty

		FROM @Root_Config ORDER BY ParentConfigIndex,ChildConfigIndex

		OPTION(MAXRECURSION 0)    
	 END


GO


GO