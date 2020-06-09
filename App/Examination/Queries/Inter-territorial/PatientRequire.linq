<Query Kind="Expression">
  <Connection>
    <ID>a935a395-cebe-4d7a-84d9-914a51b43e82</ID>
    <Persist>true</Persist>
    <Server>.\SQLEXPRESS</Server>
    <Database>medicine_ins</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

FactPatients.Where(p=>p.AccountId == 18).Where(p=>
((p.InsuranceDocNumber == null || p.InsuranceDocNumber == string.Empty)&&(p.INP == null || p.INP == string.Empty)) ||
p.InsuranceDocType == null)
.Select(p=>p.PatientId)