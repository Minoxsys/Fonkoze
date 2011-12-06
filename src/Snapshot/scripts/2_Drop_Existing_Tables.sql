    
go alter  table PermissionRoles  drop foreign key FK_PermissionRoles_RoleId


    
go alter  table PermissionRoles  drop foreign key FK_PermissionRoles_PermissionId


    
go alter  table RoleUsers  drop foreign key FK_RoleUsers_UserId


    
go alter  table RoleUsers  drop foreign key FK_RoleUsers_RoleId


    
go alter  table Addendums  drop foreign key FK_Addendums_IndividualWorkContractId


    
go alter table BankAccounts  drop foreign key FK_Banks



go alter  table Candidates  drop foreign key FK_Candidates_CurrencyId


    
go alter  table Candidates  drop foreign key FK_Candidates_EmployeeId


    
go alter  table Candidates  drop foreign key FK_Candidates_DepartmentId


    
go alter  table Candidates  drop foreign key FK_Candidates_ComputerTypeId


    
go alter  table Candidates  drop foreign key FK_Candidates_WorkPositionId


    
go alter table CertificateOfServices  drop foreign key FK_Employee_CertificateOfServices



go alter  table Discussions  drop foreign key FK_Discussions_EmployeeId


    
go alter  table Employees  drop foreign key FK_Employees_ContactInformationId


    
go alter  table Employees  drop foreign key FK_Employees_AddressInformationId


    
go alter  table ExchangeRates  drop foreign key FK_ExchangeRates_CurrencyTypesId


    
go alter  table ExpenseReports  drop foreign key FK_ExpenseReports_CurrencyTypesId


    
go alter  table ExpenseReports  drop foreign key FK_ExpenseReports_ExpenseTypesId


    
go alter  table ExpenseReports  drop foreign key FK_ExpenseReports_EmployeesId


    
go alter  table GeneralCertificates  drop foreign key FK_GeneralCertificates_IndividualWorkContractsId


    
go alter  table IdCards  drop foreign key FK_IdCards_IdentificationCardId


    
go alter  table IdCards  drop foreign key FK_IdCards_PassportId


    
go alter  table IdCards  drop foreign key FK_IdCards_DrivingLicenseId


    
go alter  table IdCards  drop foreign key FK_IdCards_EmployeeId



go alter table PfaCertificates  drop foreign key FK_Bank


    
go alter table PfaCertificates  drop foreign key FK_CAENCodes


    
go alter table PfaCertificates  drop foreign key FK_Employee


    
go alter  table Tasks  drop foreign key FK_IdCards_AssignedUserId


    go drop table if exists Permissions

    go drop table if exists PermissionRoles

    go drop table if exists Roles

    go drop table if exists RoleUsers

    go drop table if exists Users

    go drop table if exists AbsenceTypes
    
    go drop table if exists ActivityTasks

    go drop table if exists Addendums

    go drop table if exists AddressInformations

    go drop table if exists Banks
    
    go drop table if exists BankAccounts

    go drop table if exists CAENCodes

    go drop table if exists Candidates
    
    go drop table if exists CertificateOfServices

    go drop table if exists Citys

    go drop table if exists ComputerTypes

    go drop table if exists ConfigurationTypes

    go drop table if exists ContactInformations

    go drop table if exists CORCodes

    go drop table if exists CurrencyTypes
    
    go drop table if exists DecisionForDissolutionOf_IWCs

    go drop table if exists Departments

    go drop table if exists Discussions

    go drop table if exists DiscussionKeywords

    go drop table if exists DrivingLicenses

    go drop table if exists Employees

    go drop table if exists ExchangeRates

    go drop table if exists ExpenseReports

    go drop table if exists ExpenseTypes

    go drop table if exists GeneralCertificates
    
    go drop table if exists HealthPFAs

    go drop table if exists HolidayRequests

    go drop table if exists IdCards

    go drop table if exists IdentificationCards

    go drop table if exists IndividualWorkContracts
    
    go drop table if exists InformationOnEmployments

    go drop table if exists MedicalCertificates

    go drop table if exists OrganigramInformations

    go drop table if exists Passports
    
    go drop table if exists PfaCertificates

    go drop table if exists PfaContracts

    go drop table if exists PfaFinances

    go drop table if exists Tasks

    go drop table if exists WorkPositions
