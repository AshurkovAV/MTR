<Query Kind="Expression">
  <Connection>
    <ID>b1996b57-fd46-492c-bef8-8d58f766e8f6</ID>
    <Persist>true</Persist>
    <Server>.\SQLEXPRESS</Server>
    <Database>medicine_ins</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

FactMedicalEvents.Where(p=>p.FactPatient.AccountId == 18).Where(p=>p.DiagnosisGeneralM001.Payable == 1).Select(p=>p.MedicalEventId)