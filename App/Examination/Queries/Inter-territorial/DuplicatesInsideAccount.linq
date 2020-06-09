<Query Kind="Expression">
  <Connection>
    <ID>b1996b57-fd46-492c-bef8-8d58f766e8f6</ID>
    <Persist>true</Persist>
    <Server>.\SQLEXPRESS</Server>
    <Database>medicine_ins</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

FactMedicalEvents.Where(p=>p.FactPatient.AccountId == 18).GroupBy( 
x => new { 
A = x.SpecialityCode, 
B = x.EventBegin, 
C = x.EventEnd,
D = x.ProfileCodeId,
E = x.FactPatient.INP,
F = x.FactPatient.InsuranceDocNumber,
G = x.FactPatient.Newborn,
H = x.FactPatient.PersonalIdFactPerson,
}, x => x )
.Where( g => g.Count() > 1 )
.SelectMany(p=>p).Select(p=>p.MedicalEventId)
//.Select(g=>g.First())
