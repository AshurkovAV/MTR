<Query Kind="Expression">
  <Connection>
    <ID>b1996b57-fd46-492c-bef8-8d58f766e8f6</ID>
    <Persist>true</Persist>
    <Server>.\SQLEXPRESS</Server>
    <Database>medicine_ins</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

FactMedicalServices.Where(p=>p.FactMedicalEvent.FactPatient.AccountId == 18).Where(p=>
p.Profile == null||
p.IsChildren == null||
p.ServiceBegin == null || 
p.ServiceEnd == null ||
p.Diagnosis == null ||
(p.Quantity == null||p.Quantity == 0)||
(p.Rate == null||p.Rate == 0)||
(p.Price == null||p.Price == 0)||
p.SpecialityCode == null ||
(p.ServiceName == null || p.ServiceName == string.Empty))
.Select(p=>p.MedicalServicesId)
