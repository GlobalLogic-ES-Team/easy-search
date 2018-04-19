Create procedure [dbo].[Insert_Persons]
	@tblPersons dbo.[PersonTableType] Readonly
as
begin
	set nocount on;
	insert into Person 
		  ( ssn,gender,firstname,lastname,email,dob,cell,salary,interests,json_data,street,state,city,zip)
	select  ssn,gender,firstname,lastname,email,dob,cell,salary,interests,json_data,street,state,city,zip
		from @tblPersons
end
RETURN 0
