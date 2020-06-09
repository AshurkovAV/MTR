<Query Kind="Expression">
  <Connection>
    <ID>b1996b57-fd46-492c-bef8-8d58f766e8f6</ID>
    <Persist>true</Persist>
    <Server>.\SQLEXPRESS</Server>
    <Database>medicine_ins</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

FactPatients.Where(p=>p.AccountId == 18).Where(p=>
p.InsuranceDocType != 3 && 
(p.PersonalIdFactPerson.FactDocuments.First().DocType == null || 
p.PersonalIdFactPerson.FactDocuments.First().DocNum == null || 
p.PersonalIdFactPerson.FactDocuments.First().DocSeries == null || 
p.PersonalIdFactPerson.FactDocuments.First().DocNum == string.Empty ||
p.PersonalIdFactPerson.FactDocuments.First().DocSeries == string.Empty)).Select(p=>p.PatientId)