
    go create table if not exists  Permissions (
         Id VARCHAR(40) not null,
       Name VARCHAR(255) unique,
       primary key (Id)
    )

    go create table if not exists  PermissionRoles(
        PermissionId_FK VARCHAR(40) not null,
       RoleId_FK VARCHAR(40) not null
    )

    go create table if not exists  Roles (
        Id VARCHAR(40) not null,
       Name VARCHAR(255) unique,
       Description VARCHAR(255),
       primary key (Id)
    )

    go create table if not exists  RoleUsers (
        RoleId_FK VARCHAR(40) not null,
       UserId_FK VARCHAR(40) not null
    )

    go create table if not exists  Users (
        Id VARCHAR(40) not null,
       UserName VARCHAR(255) unique,
       primary key (Id)
    )

    go create table if not exists  AbsenceTypes (
        Id VARCHAR(40) not null,
       Type VARCHAR(255) unique,
       Description VARCHAR(255),
       primary key (Id)
    )
	
	go create table if not exists ActivityTasks (
        Id VARCHAR(40) not null,
       Category VARCHAR(255),
       Title VARCHAR(255),
       Description VARCHAR(255),
       CandidateId VARCHAR(40),
       CandidateName VARCHAR(255),
       EmployeeId VARCHAR(40),
       EmployeeName VARCHAR(255),
       Date DATETIME,
       Resolved TINYINT(1),
       CommandId VARCHAR(40),
       primary key (Id)
    )

    go create table if not exists  Addendums (
        Id VARCHAR(40) not null,
       DateAdded DATETIME,
       RegNo INTEGER,
       RegDate DATETIME,
       ModificationStartDate DATETIME,
       SalaryFrom DOUBLE,
       SalaryTo DOUBLE,
       SalaryIncreaseFrom DOUBLE,
       SalaryIncreaseTo DOUBLE,
       Profession VARCHAR(255),
       ContractDurationFrom VARCHAR(255),
       ContractDurationTo VARCHAR(255),
       WorkHoursFromHoursADay INTEGER,
       WorkHoursFromHoursAWeek INTEGER,
       WorkHoursToHoursADay INTEGER,
       WorkHoursToHoursAWeek INTEGER,
       AbsenceDaysFrom INTEGER,
       AbsenceDaysTo INTEGER,
       ExtraAbsenceDaysFrom INTEGER,
       ExtraAbsenceDaysTo INTEGER,
       ActivityWorkPlace VARCHAR(255),
       WorkConditionsFrom VARCHAR(255),
       WorkConditionsTo VARCHAR(255),
       OtherModifications VARCHAR(255),
       SalaryIncreaseType VARCHAR(255),
       SalaryIncreaseFinal DOUBLE,
       IndividualWorkContract_FK VARCHAR(40),
       primary key (Id)
    )

    go create table if not exists  AddressInformations (
        Id VARCHAR(40) not null,
       Municipality VARCHAR(255),
       Address VARCHAR(255),
       Country VARCHAR(255),
       County VARCHAR(255),
       primary key (Id)
    )

    go create table if not exists  Banks (
        Id VARCHAR(40) not null,
       Name VARCHAR(255) unique,
       primary key (Id)
    )
	
	go create table if not exists BankAccounts (
        Id VARCHAR(40) not null,
       IBAN VARCHAR(255),
       EmployeeName VARCHAR(255),
       EmployeeId VARCHAR(40),
       BankName_FK VARCHAR(40),
       primary key (Id)
    )

    go create table if not exists  CAENCodes (
        Id VARCHAR(40) not null,
       Name VARCHAR(255) unique,
       primary key (Id)
    )

    go create table if not exists  Candidates (
	    Id VARCHAR(40) not null,
       Tasks LONGTEXT,
       Name VARCHAR(255),
       Telephone VARCHAR(255),
       PersonalEmail VARCHAR(255),
       DeskPosition VARCHAR(255),
       OperatingSystem VARCHAR(255),
       EvozonEmail VARCHAR(255),
       Emails VARCHAR(255),
       Money DOUBLE,
       RenegocioateAfter INTEGER,
       StartDate DATETIME,
       WorkTime VARCHAR(255),
       WorkScheduleFrom INTEGER,
       WorkScheduleUntil INTEGER,
       WorkPeriodIsDefinite TINYINT(1),
       WorkPeriod_DeterminedDate DATETIME,
       ProbationPeriod INTEGER,
       NoticePeriod INTEGER,
       Department_FK VARCHAR(40),
       Manager_FK VARCHAR(40),
       ComputerType_FK VARCHAR(40),
       WorkPosition_FK VARCHAR(40),
       Currency_FK VARCHAR(40),
       primary key (Id)
    )
	
	go create table if not exists CertificateOfServices (
        Id VARCHAR(40) not null,
       EmployementInformations LONGTEXT,
       CIMNo VARCHAR(255),
       CIMRegistrationDate DATETIME,
       LastDayOfWork DATETIME,
       FirstDayOfWork DATETIME,
       EndDateOfCIM DATETIME,
       YearlyAbsenceDaysNoPaid VARCHAR(255),
       UnfoundedAbsenceDays VARCHAR(255),
       Article VARCHAR(255),
       CNP VARCHAR(255),
       Employee_FK VARCHAR(40),
       primary key (Id)
    )

    go create table if not exists  Citys (
        Id VARCHAR(40) not null,
       Name VARCHAR(255) unique,
       primary key (Id)
    )

    go create table if not exists  ComputerTypes (
        Id VARCHAR(40) not null,
       Name VARCHAR(255) unique,
       primary key (Id)
    )

    go create table if not exists  ConfigurationTypes (
        Id VARCHAR(40) not null,
       Name VARCHAR(255),
       Description VARCHAR(255),
       Controller VARCHAR(255),
       primary key (Id)
    )

    go create table if not exists  ContactInformations (
        Id VARCHAR(40) not null,
       SkypeId VARCHAR(255),
       YahooId VARCHAR(255),
       EvozonMail VARCHAR(255),
       PersonalMail VARCHAR(255),
       Phone VARCHAR(255),
       ContactPersonName VARCHAR(255),
       ContactPersonPhone VARCHAR(255),
       primary key (Id)
    )

    go create table if not exists  CORCodes (
        Id VARCHAR(40) not null,
       Name VARCHAR(255) unique,
       primary key (Id)
    )

    go create table if not exists  CurrencyTypes (
        Id VARCHAR(40) not null,
       Name VARCHAR(255) unique,
       primary key (Id)
    )
	
	go create table if not exists DecisionForDissolutionOf_IWCs (
        Id VARCHAR(40) not null,
       Number VARCHAR(255),
       Date DATETIME,
       RequestOfResignation VARCHAR(255),
       UnderArticle VARCHAR(255),
       ReasonsOfResignation VARCHAR(255),
       StartDate DATETIME,
       EmployeeId VARCHAR(40),
       EmployeeName VARCHAR(255),
       CNP VARCHAR(255),
       ITM_NO VARCHAR(255),
       ITM_Date DATETIME,
       DatePeriodFrom DATETIME,
       DatePeriodUntil DATETIME,
       CFS INTEGER,
       UnjustifiedAbsences INTEGER,
       TechnicalUnemploymentPeriod VARCHAR(255),
       primary key (Id)
    )

    go create table if not exists  Departments (
        Id VARCHAR(40) not null,
       Name VARCHAR(255) unique,
       primary key (Id)
    )

    go create table if not exists  Discussions (
        Id VARCHAR(40) not null,
       Keywords VARCHAR(255),
       Date DATETIME,
       Notes VARCHAR(255),
       Employee_FK VARCHAR(40),
       primary key (Id)
    )

    go create table if not exists  DiscussionKeywords (
        Id VARCHAR(40) not null,
       Name VARCHAR(255) unique,
       primary key (Id)
    )

    go create table if not exists  DrivingLicenses (
        Id VARCHAR(40) not null,
       DriverLicenseNo VARCHAR(255),
       IssueDate DATETIME,
       ExpiryDate DATETIME,
       primary key (Id)
    )

    go create table if not exists  Employees ( 
	    Id VARCHAR(40) not null,
       Tasks LONGTEXT,
       InformationFromIWC LONGTEXT,
       Name VARCHAR(255),
       Birthday DATETIME,
       IsOffboarding TINYINT(1),
       BirthdayCity VARCHAR(255),
       BirthdayCounty VARCHAR(255),
       BirthdayCountry VARCHAR(255),
       PhotoPath VARCHAR(255),
       ContactInformation_FK VARCHAR(40),
       AddressInformation_FK VARCHAR(40),
       primary key (Id)
    )

    go create table if not exists  ExchangeRates (
        Id VARCHAR(40) not null,
       ExchangeRateValue DOUBLE,
       Date DATETIME,
       Currency_FK VARCHAR(40),
       primary key (Id)
    )

    go create table if not exists  ExpenseReports (
        Id VARCHAR(40) not null,
       Description VARCHAR(255),
       Cost DOUBLE,
       Date DATETIME,
       Currency_FK VARCHAR(40),
       ExpenseType_FK VARCHAR(40),
       Employee_FK VARCHAR(40),
       primary key (Id)
    )

    go create table if not exists  ExpenseTypes (
        Id VARCHAR(40) not null,
       Description VARCHAR(255) unique,
       primary key (Id)
    )

    go create table if not exists  GeneralCertificates (
        Id VARCHAR(40) not null,
       Reason VARCHAR(255),
       RegNo INTEGER,
       RegDate DATETIME,
       Contract_FK VARCHAR(40),
       primary key (Id)
    )
	
	go create table if not exists HealthPFAs (
        Id VARCHAR(40) not null,
       EmployeeId VARCHAR(40),
       EmployeeName VARCHAR(255),
       City VARCHAR(255),
       County VARCHAR(255),
       Address VARCHAR(255),
       CNP VARCHAR(255),
       IdNo VARCHAR(255),
       IdSeries VARCHAR(255),
       IssuedBy VARCHAR(255),
       IssueDate DATETIME,
       EmpoweredPersons_Name VARCHAR(255),
       EmpoweredPersons_CNP VARCHAR(255),
       EmpoweredPersons_IdNo VARCHAR(255),
       EmpoweredPersons_IdSeries VARCHAR(255),
       HealthHouse_DepositTo VARCHAR(255),
       PFACertificateId VARCHAR(40),
       PFAName VARCHAR(255),
       CUI VARCHAR(255),
       primary key (Id)
    )

    go create table if not exists  HolidayRequests (
        Id VARCHAR(40) not null,
       StartDate DATETIME,
       EndDate DATETIME,
       NoDays INTEGER,
       EmployeeId VARCHAR(40),
       AbsenceTypeId VARCHAR(40),
       primary key (Id)
    )

    go create table if not exists  IdCards (
        Id VARCHAR(40) not null,
       IdentificationCard_FK VARCHAR(40),
       Passport_FK VARCHAR(40),
       DrivingLicense_FK VARCHAR(40),
       Employee_FK VARCHAR(40),
       primary key (Id)
    )

    go create table if not exists  IdentificationCards (
        Id VARCHAR(40) not null,
       CNP VARCHAR(255),
       IdNo VARCHAR(255),
       IdSeries VARCHAR(255),
       IssuedBy VARCHAR(255),
       IssueDate DATETIME,
       ExpiryDate DATETIME,
       primary key (Id)
    )

    go create table if not exists  IndividualWorkContracts (
        Id VARCHAR(40) not null,
       RegNo INTEGER,
       RegDate DATETIME,
       ElectronicRegisterNo INTEGER,
       EmployeeId VARCHAR(40),
       EmployeeName VARCHAR(255),
       EmployeeMunicipality VARCHAR(255),
       EmployeeAddress VARCHAR(255),
       EmployeeCounty VARCHAR(255),
       EmployeeIdCardSeries VARCHAR(255),
       EmployeeIdCardNo VARCHAR(255),
       EmployeeIdCardIssuedBy VARCHAR(255),
       EmployeeIdCardIssueDate VARCHAR(255),
       EmployeeIdCardCNP VARCHAR(255),
       WorkAuthorizationSeries VARCHAR(255),
       WorkAuthorizationNo VARCHAR(255),
       WorkAuthorizationDate VARCHAR(255),
       Purpose VARCHAR(255),
       ContractDurationIsDetermined TINYINT(1),
       UndeterminedContractDurationStartDate DATETIME,
       ContractDurationStartDate DATETIME,
       ContractDurationPeriod INTEGER,
       ContractDurationEndDate DATETIME,
       ContractDurationLawArticleLetter VARCHAR(255),
       WorkPlaceLocation VARCHAR(255),
       WorkPlaceAlternate VARCHAR(255),
       Profession VARCHAR(255),
       WorkDurationHoursADay INTEGER,
       WorkDurationHoursAWeek INTEGER,
       WorkDurationFractionHoursADay INTEGER,
       WorkDurationFractionHoursAWeek INTEGER,
       WorkDurationFromHoursADay INTEGER,
       WorkDurationToHoursADay INTEGER,
       WorkDurationFromHoursANight INTEGER,
       WorkDurationToHoursANight INTEGER,
       YearlyAbsenceDays INTEGER,
       ExtraAbsenceDays INTEGER,
       SalaryQuanta DOUBLE,
       SalaryCurrency VARCHAR(255),
       SalaryIncreaseType VARCHAR(255),
       SalaryIncrease DOUBLE,
       SalaryAllowance DOUBLE,
       SalaryAdditionalBenefits DOUBLE,
       SalaryAdditionalBenefitsRecompensationMethod VARCHAR(255),
       SalaryOtherAdditions VARCHAR(255),
       SalaryPayDay INTEGER,
       IndividualProtectionEquipment VARCHAR(255),
       IndividualWorkEquipment VARCHAR(255),
       HygenicalMaterials VARCHAR(255),
       ProtectionAlimentation VARCHAR(255),
       OtherProtections VARCHAR(255),
       ProbationPeriodDuration INTEGER,
       FiringNoticePeriod INTEGER,
       QuittingNoticePeriod INTEGER,
       OtherClauses VARCHAR(255),
       FinalDispositonsRegNo INTEGER,
       FinalDispositonsRegDate DATETIME,
       FinalDispositonsInspectorateMunicipality VARCHAR(255),
       Employer VARCHAR(255),
       primary key (Id)
    )
	
	go create table if not exists InformationOnEmployments (
        Id VARCHAR(40) not null,
       EmployeeId VARCHAR(40),
       EmployeeName VARCHAR(255),
       Headquarters VARCHAR(255),
       HeadquartersAddress VARCHAR(255),
       CORCode VARCHAR(255),
       JobRisks VARCHAR(255),
       StartDate DATETIME,
       ContractDurationIsDetermined TINYINT(1),
       ContractDurationStartDate DATETIME,
       ContractDurationEndDate DATETIME,
       NumberOfDaysForRestHoliday INTEGER,
       AdditionalNumberOfDaysForRestHoliday INTEGER,
       NoticeConditions VARCHAR(255),
       Salary VARCHAR(255),
       DateForBankTransfer VARCHAR(255),
       WorkDurationHoursADay INTEGER,
       WorkDurationHoursAWeek INTEGER,
       StartHourInADay INTEGER,
       EndHourInADay INTEGER,
       ContractIndicationForWorkingConditions VARCHAR(255),
       ProbationPeriodDuration INTEGER,
       Date DATETIME,
       ContractDurationPeriod INTEGER,
       primary key (Id)
    )

    go create table if not exists  MedicalCertificates (
        Id VARCHAR(40) not null,
       RegistrationNumber INTEGER,
       Date DATETIME,
       NumberOfSickDays INTEGER,
       NumberOfMonths INTEGER,
       EmployeeId VARCHAR(40),
       EmployeeName VARCHAR(255),
       EmployeeCity VARCHAR(255),
       EmployeeCounty VARCHAR(255),
       EmployeeCountry VARCHAR(255),
       EmployeeAddress VARCHAR(255),
       EmployeeIdCardSeries VARCHAR(255),
       EmployeeIdCardNo VARCHAR(255),
       EmployeeIdCardIssuedBy VARCHAR(255),
       EmployeeIssueDate DATETIME,
       EmployeeExpiryDate DATETIME,
       EmployeeIdCardCNP VARCHAR(255),
       EmployeeRegNo INTEGER,
       EmployeeRegDate DATETIME,
       EmployeeContractDurationIsDetermined TINYINT(1),
       EmployeeUndeterminedContractDurationStartDate DATETIME,
       EmployeeContractDurationStartDate DATETIME,
       EmployeeWorkDurationHoursADay INTEGER,
       EmployeeWorkDurationFractionHoursADay INTEGER,
       primary key (Id)
    )

    go create table if not exists  OrganigramInformations (
        Id VARCHAR(40) not null,
       EmployeeId VARCHAR(40),
       DepartmentId VARCHAR(40),
       ManagerId VARCHAR(40),
       WorkPositionId VARCHAR(40),
       primary key (Id)
    )

    go create table if not exists  Passports (
        Id VARCHAR(40) not null,
       PassportNo VARCHAR(255),
       Category VARCHAR(255),
       IssueDate DATETIME,
       ExpiryDate DATETIME,
       primary key (Id)
    )
	
	go create table if not exists PfaCertificates (
        Id VARCHAR(40) not null,
       IBAN VARCHAR(255),
       PFAName VARCHAR(255),
       City VARCHAR(255),
       Address VARCHAR(255),
       Country VARCHAR(255),
       County VARCHAR(255),
       CUI VARCHAR(255),
       RegistrationDate DATETIME,
       VATNumber VARCHAR(255),
       TradeRegisterNo VARCHAR(255),
       IssueDate DATETIME,
       HealthHome VARCHAR(255),
       Series VARCHAR(255),
       Number VARCHAR(255),
       FinancialAdministration VARCHAR(255),
       Bank_FK VARCHAR(40),
       CAENCode_FK VARCHAR(40),
       Employee_FK VARCHAR(40),
       primary key (Id)
    )
	
	go create table if not exists PfaContracts (
        Id VARCHAR(40) not null,
       EmployeeName VARCHAR(255),
       EmployeeCity VARCHAR(255),
       EmployeeAddress VARCHAR(255),
       EmployeeCounty VARCHAR(255),
       EmployeeId VARCHAR(40),
       FinancialAdministration VARCHAR(255),
       TradeRegisterNumber VARCHAR(255),
       CUI VARCHAR(255),
       BankName VARCHAR(255),
       IBAN VARCHAR(255),
       Email VARCHAR(255),
       PFAName VARCHAR(255),
       PFACity VARCHAR(255),
       PFAAddress VARCHAR(255),
       PFACounty VARCHAR(255),
       VATNumber VARCHAR(255),
       ContractSubject VARCHAR(255),
       Series VARCHAR(255),
       Number VARCHAR(255),
       IssuedBy VARCHAR(255),
       IssueDate DATETIME,
       CNP VARCHAR(255),
       PFAContractRegisterDate DATETIME,
       RegistrationNo VARCHAR(255),
       PFASalary DOUBLE,
       primary key (Id)
    )
    
    go create table if not exists PfaFinances (
        Id VARCHAR(40) not null,
       EmployeeName VARCHAR(255),
       City VARCHAR(255),
       Country VARCHAR(255),
       County VARCHAR(255),
       Address VARCHAR(255),
       IdSeries VARCHAR(255),
       IdNo VARCHAR(255),
       CNP VARCHAR(255),
       IssuedBy VARCHAR(255),
       IssueDate DATETIME,
       PfaName VARCHAR(255),
       CUI VARCHAR(255),
       RegistrationCodeForTVA VARCHAR(255),
       EmpoweredPersons_Name VARCHAR(255),
       EmpoweredPersons_CNP VARCHAR(255),
       EmpoweredPersons_IdNo VARCHAR(255),
       EmpoweredPersons_IdSeries VARCHAR(255),
       ReasonOfEmpowerment VARCHAR(255),
       FinancialAdministration VARCHAR(255),
       EmployeeId VARCHAR(40),
       PfaCertificateId VARCHAR(40),
       primary key (Id)
    )

    go create table if not exists  Tasks (
        Id VARCHAR(40) not null,
       TaskCategory VARCHAR(255),
       TaskGroup VARCHAR(255),
       TaskOrder INTEGER,
       Title VARCHAR(255),
       TaskDescription VARCHAR(255),
       AssignedName VARCHAR(255),
       DueInDays INTEGER,
       Command VARCHAR(255),
       AssignedUser_FK VARCHAR(40),
       primary key (Id)
    )

    go create table if not exists  WorkPositions (
        Id VARCHAR(40) not null,
       Name VARCHAR(255) unique,
       primary key (Id)
    )

    go alter table PermissionRoles
        add index (RoleId_FK), 
        add constraint FK_PermissionRoles_RoleId 
        foreign key (RoleId_FK) 
        references Roles (Id)

    go alter table PermissionRoles 
        add index (PermissionId_FK), 
        add constraint FK_PermissionRoles_PermissionId 
        foreign key (PermissionId_FK) 
        references Permissions (Id)

    go alter table RoleUsers 
        add index (UserId_FK), 
        add constraint FK_RoleUsers_UserId 
        foreign key (UserId_FK) 
        references Users (Id)

    go alter table RoleUsers 
        add index (RoleId_FK), 
        add constraint FK_RoleUsers_RoleId 
        foreign key (RoleId_FK) 
        references Roles (Id)

    go alter table Addendums 
        add index (IndividualWorkContract_FK), 
        add constraint FK_Addendums_IndividualWorkContractId 
        foreign key (IndividualWorkContract_FK) 
        references IndividualWorkContracts (Id)

	go alter table BankAccounts 
		add index (BankName_FK),
        add constraint FK_Banks 
        foreign key (BankName_FK) 
        references Banks (Id)
        
    go alter table Candidates 
        add index (Department_FK), 
        add constraint FK_Candidates_DepartmentId 
        foreign key (Department_FK) 
        references Departments (Id)
    
    go alter table Candidates 
        add index (Manager_FK), 
        add constraint FK_Candidates_EmployeeId 
        foreign key (Manager_FK) 
        references Employees (Id)

    go alter table Candidates 
        add index (ComputerType_FK), 
        add constraint FK_Candidates_ComputerTypeId 
        foreign key (ComputerType_FK) 
        references ComputerTypes (Id)

    go alter table Candidates 
        add index (WorkPosition_FK), 
        add constraint FK_Candidates_WorkPositionId 
        foreign key (WorkPosition_FK) 
        references WorkPositions (Id)
        
    go alter table Candidates 
        add index (Currency_FK), 
        add constraint FK_Candidates_CurrencyId 
        foreign key (Currency_FK) 
        references CurrencyTypes (Id)
    
    go alter table CertificateOfServices
    	add index (Employee_FK),
        add constraint FK_Employee_CertificateOfServices 
        foreign key (Employee_FK) 
        references Employees (Id)

    go alter table Discussions 
        add index (Employee_FK), 
        add constraint FK_Discussions_EmployeeId 
        foreign key (Employee_FK) 
        references Employees (Id)

    go alter table Employees 
        add index (ContactInformation_FK), 
        add constraint FK_Employees_ContactInformationId 
        foreign key (ContactInformation_FK) 
        references ContactInformations (Id)

    go alter table Employees 
        add index (AddressInformation_FK), 
        add constraint FK_Employees_AddressInformationId 
        foreign key (AddressInformation_FK) 
        references AddressInformations (Id)

    go alter table ExchangeRates 
        add index (Currency_FK), 
        add constraint FK_ExchangeRates_CurrencyTypesId 
        foreign key (Currency_FK) 
        references CurrencyTypes (Id)

    go alter table ExpenseReports 
        add index (Currency_FK), 
        add constraint FK_ExpenseReports_CurrencyTypesId 
        foreign key (Currency_FK) 
        references CurrencyTypes (Id)

    go alter table ExpenseReports 
        add index (ExpenseType_FK), 
        add constraint FK_ExpenseReports_ExpenseTypesId 
        foreign key (ExpenseType_FK) 
        references ExpenseTypes (Id)

    go alter table ExpenseReports 
        add index (Employee_FK), 
        add constraint FK_ExpenseReports_EmployeesId 
        foreign key (Employee_FK) 
        references Employees (Id)

    go alter table GeneralCertificates 
        add index (Contract_FK), 
        add constraint FK_GeneralCertificates_IndividualWorkContractsId 
        foreign key (Contract_FK) 
        references IndividualWorkContracts (Id)

    go alter table IdCards 
        add index (IdentificationCard_FK), 
        add constraint FK_IdCards_IdentificationCardId 
        foreign key (IdentificationCard_FK) 
        references IdentificationCards (Id)

    go alter table IdCards 
        add index (Passport_FK), 
        add constraint FK_IdCards_PassportId 
        foreign key (Passport_FK) 
        references Passports (Id)

    go alter table IdCards 
        add index (DrivingLicense_FK), 
        add constraint FK_IdCards_DrivingLicenseId 
        foreign key (DrivingLicense_FK) 
        references DrivingLicenses (Id)

    go alter table IdCards 
        add index (Employee_FK), 
        add constraint FK_IdCards_EmployeeId 
        foreign key (Employee_FK) 
        references Employees (Id)
        
    go alter table PfaCertificates 
    	add index (Bank_FK),
        add constraint FK_Bank 
        foreign key (Bank_FK) 
        references Banks (Id)
    
    go alter table PfaCertificates 
    	add index (CAENCode_FK),
        add constraint FK_CAENCodes 
        foreign key (CAENCode_FK) 
        references CAENCodes (Id)
    
    go alter table PfaCertificates 
    	add index (Employee_FK),
    	add constraint FK_Employee
    	foreign key (Employee_FK)
    	references Employees (Id)

    go alter table Tasks 
        add index (AssignedUser_FK), 
        add constraint FK_IdCards_AssignedUserId 
        foreign key (AssignedUser_FK) 
        references Users (Id)
		
	go alter table PermissionRoles
		add Unique index (`PermissionId_FK`, `RoleId_FK`)
		
	go alter table RoleUsers
		add Unique index (`RoleId_FK`, `UserId_FK`)
	
	
	-- INSERT INTO
	
	-- DiscussionKeywords
go INSERT IGNORE INTO DiscussionKeywords
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'Salary Raise')

go INSERT IGNORE INTO DiscussionKeywords
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'Client Feedback')
		    
go INSERT IGNORE INTO DiscussionKeywords
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'Personal Development Plan')
       
       
-- CORCodes
 go INSERT IGNORE INTO CORCodes
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'programator  213102')
           
 go INSERT IGNORE INTO CORCodes
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'programator ajutor  312101')
 
 go INSERT IGNORE INTO CORCodes
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'director general societate comerciala 121011')
 
 go INSERT IGNORE INTO CORCodes
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'inginer de sistem 213901')
 
 go INSERT IGNORE INTO CORCodes
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'tehnician electronica, transporturi, posta si telecomunicatii 311406')
 
 go INSERT IGNORE INTO CORCodes
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'ilustrator muzical 245307')
 
 go INSERT IGNORE INTO CORCodes
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'manager proiect  241919')
 
 go INSERT IGNORE INTO CORCodes
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'analist resurse umane 342306')
           
 go INSERT IGNORE INTO CORCodes
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'designer pagini web 245211')
 
 go INSERT IGNORE INTO CORCodes
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'secretara 411501')
 
 go INSERT IGNORE INTO CORCodes
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'manager resurse umane 123207')
           
 go INSERT IGNORE INTO CORCodes
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'director economic 121020')
 
 go INSERT IGNORE INTO CORCodes
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'referent economist in economie generala 244102')
 
 go INSERT IGNORE INTO CORCodes
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'economist sef 121102')
           
           
-- AbsenceTypes           
 go INSERT IGNORE INTO AbsenceTypes
           (`Id`
           ,`Description`
           ,`Type`)
     VALUES
           (UUID()
           ,N'Concediu Restant'
           ,N'CR')
    
     go INSERT IGNORE INTO AbsenceTypes
           (`Id`
           ,`Description`
           ,`Type`)
     VALUES
           (UUID()
           ,N'Concediu Odihna'
           ,N'CO')
           
    go INSERT IGNORE INTO AbsenceTypes
           (`Id`
           ,`Description`
           ,`Type`)
     VALUES
           (UUID()
           ,N'Concediu Fara Salar'
           ,N'CFS')
   
    go INSERT IGNORE INTO AbsenceTypes
           (`Id`
           ,`Description`
           ,`Type`)
     VALUES
           (UUID()
           ,N'Concediu Medical'
           ,N'CM')
           
                  
   go INSERT IGNORE INTO AbsenceTypes
           (`Id`
           ,`Description`
           ,`Type`)
     VALUES
           (UUID()
           ,N'Situatii Speciale'
           ,N'SP')
           
	go INSERT IGNORE INTO AbsenceTypes
           (`Id`
           ,`Description`
           ,`Type`)
     VALUES
           (UUID()
           ,N'Absente Nemotivate'
           ,N'AN')	
           
           
-- ComputerTypes 		   
go INSERT IGNORE INTO ComputerTypes
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'Laptop')  
           
go INSERT IGNORE INTO ComputerTypes
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'Desktop')
           
           
--  CurrencyTypes         
 go INSERT IGNORE INTO CurrencyTypes
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'EUR')
		   
 go INSERT IGNORE INTO CurrencyTypes
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'RON')
		   
		   
 go INSERT IGNORE INTO CurrencyTypes
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'USD')

  go INSERT IGNORE INTO CurrencyTypes
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'GBP')
		   

 go INSERT IGNORE INTO CurrencyTypes
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'AUD')


-- WorkPositions
go INSERT IGNORE INTO WorkPositions
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'Programmer')
 go INSERT IGNORE INTO WorkPositions
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'Manager')
 go INSERT IGNORE INTO WorkPositions
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'Accountant')

 go INSERT IGNORE INTO WorkPositions
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'Secretary')
           
go INSERT IGNORE INTO WorkPositions
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'IT Support')
           
           
           
-- 	ExpenseTypes	   
  go INSERT IGNORE INTO ExpenseTypes
           (`Id`
           ,`Description`)
     VALUES
           (UUID()
           ,N'Plane')
 go INSERT IGNORE INTO ExpenseTypes
           (`Id`
           ,`Description`)
     VALUES
           (UUID()
           ,N'Taxi')

  go INSERT IGNORE INTO ExpenseTypes
           (`Id`
           ,`Description`)
     VALUES
           (UUID()
           ,N'Train ride')
		   
 go INSERT IGNORE INTO ExpenseTypes
           (`Id`
           ,`Description`)
     VALUES
           (UUID()
           ,N'Hotel lodging')
 go INSERT IGNORE INTO ExpenseTypes
           (`Id`
           ,`Description`)
     VALUES
           (UUID()
           ,N'Baggage wrapping')
           
           
           
-- Departments		   
 go INSERT IGNORE INTO Departments
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'Java')

    go INSERT IGNORE INTO Departments
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'.NET')
           
    go INSERT IGNORE INTO Departments
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'PHP')
           
      go INSERT IGNORE INTO Departments
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'Mobile')
           
    go INSERT IGNORE INTO Departments
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'Perl')       
           
           
           
 -- ConfigurationTypes                             
 go INSERT IGNORE INTO ConfigurationTypes
           (`Id`
           ,`Name`
           ,`Description`
           ,`Controller`)
     VALUES
           ('268C5091-6923-4321-9D94-E0859C9F943F'
           ,N'COR Codes'
           ,N'Classification of Occupations in Romania'
           ,N'CORCode')
    go INSERT IGNORE INTO ConfigurationTypes
           (`Id`
           ,`Name`
           ,`Description`
           ,`Controller`)
     VALUES
           ('88B8A292-A647-48EC-AFAC-184F5C26BBCF'
           ,N'Exchange Rates'
           ,N'The exchange rate that was at a certain date'
           ,N'ExchangeRate')
     go INSERT IGNORE INTO ConfigurationTypes
           (`Id`
           ,`Name`
           ,`Description`
           ,`Controller`)
     VALUES
           ('13D32AB5-0CF7-4B65-9D4B-195CFFB8962B'
           ,N'Cities'
           ,N'Places in which employees or candidates can stay'
           ,N'City')
    go INSERT IGNORE INTO ConfigurationTypes
           (`Id`
           ,`Name`
           ,`Description`
           ,`Controller`)
     VALUES
           ('81056780-7714-432E-9A1C-113912DCD036'
           ,N'Departments'
           ,N'All the departments that the company has'
           ,N'Department')
     go INSERT IGNORE INTO ConfigurationTypes
           (`Id`
           ,`Name`
           ,`Description`
           ,`Controller`)
     VALUES
           ('01B4CEDE-347F-4EB3-A6BF-B9C59C9AF67F'
           ,N'Computer Types'
           ,N'Types of computers that an employee can work on'
           ,N'ComputerTypes')
     go INSERT IGNORE INTO ConfigurationTypes
           (`Id`
           ,`Name`
           ,`Description`
           ,`Controller`)
     VALUES
           ('A73E2BF9-D099-4D4C-B83F-D718B33D4255'
           ,N'Absence Types'
           ,N'Ways an employee can miss work'
           ,N'AbsenceTypes')
     go INSERT IGNORE INTO ConfigurationTypes
           (`Id`
           ,`Name`
           ,`Description`
           ,`Controller`)
     VALUES
           ('F61922B2-283F-4F10-A3EE-AFCF214BF52C'
           ,N'Banks'
           ,N'Names of the banks used'
           ,N'Bank')
     go INSERT IGNORE INTO ConfigurationTypes
           (`Id`
           ,`Name`
           ,`Description`
           ,`Controller`)
     VALUES
           ('08716BFB-F36D-4244-949C-FDC7D29CAE21'
           ,N'Work Positions'
           ,N'Positions that an employee can have'
           ,N'WorkPositions')
    go INSERT IGNORE INTO ConfigurationTypes
           (`Id`
           ,`Name`
           ,`Description`
           ,`Controller`)
     VALUES
           ('1146938A-BBD3-425F-8A8F-512723D4F421'
           ,N'Currencies'
           ,N'The most use currencies'
           ,N'Currencies')
    go INSERT IGNORE INTO ConfigurationTypes
           (`Id`
           ,`Name`
           ,`Description`
           ,`Controller`)
     VALUES
           ('671DC9DA-3216-47D3-8297-5B246D25801D'
           ,N'Expense Types'
           ,N'Types of expenses that an employee had committed and which are reimbursed by the company'
           ,N'ExpenseTypes')
           
     go INSERT IGNORE INTO ConfigurationTypes
           (`Id`
           ,`Name`
           ,`Description`
           ,`Controller`)
     VALUES
           ('1F6E0F14-0D28-41EF-8F56-FFC868BF45B5'
           ,N'Discussion Keywords'
           ,N'The most used keywords for a discussion with an employee'
           ,N'Discussion') 
    go INSERT IGNORE INTO ConfigurationTypes
           (`Id`
           ,`Name`
           ,`Description`
           ,`Controller`)
     VALUES
           ('5688AD06-969A-4C54-93CE-7750B67F6DE9'
           ,N'CAEN Codes'
           ,N'The principal activity of an employee'
           ,N'CAENCode')    
		   

-- CAENCodes 
	go INSERT IGNORE INTO CAENCodes
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'6201')  
           
    go INSERT IGNORE INTO CAENCodes
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'6203')  

	go INSERT IGNORE INTO CAENCodes
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'6209') 

	go INSERT IGNORE INTO CAENCodes
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'6311') 
           
-- Banks

	go INSERT IGNORE INTO Banks
           (`Id`
           ,`Name`)
     VALUES
           (UUID()
           ,N'BRD')            