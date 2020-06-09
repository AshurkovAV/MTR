<Query Kind="Expression">
  <Connection>
    <ID>a935a395-cebe-4d7a-84d9-914a51b43e82</ID>
    <Persist>true</Persist>
    <Server>.\SQLEXPRESS</Server>
    <Database>medicine</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

FactMedicalEvents.Where(p=>p.RegisterId == 10).Where(p=>
p.AssistanceConditions == null ||
p.AssistanceType == null ||
p.Department == null ||
p.ProfileCodeId == null ||
p.IsChildren == null ||
(p.History == null || p.History == string.Empty) ||
p.EventBegin == null ||
p.EventEnd == null ||
p.DiagnosisGeneral == null ||
p.Result == null ||
p.Outcome == null ||
p.SpecialityCode == null ||
p.PaymentMethod == null ||
p.Quantity == null ||
p.Rate == null ||
p.PaymentStatus == null ||
(p.Price == null||p.Price == 0)).Select(p=>p.MedicalEventId)