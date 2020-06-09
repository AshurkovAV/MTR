<Query Kind="Expression">
  <Connection>
    <ID>a935a395-cebe-4d7a-84d9-914a51b43e82</ID>
    <Persist>true</Persist>
    <Server>.\SQLEXPRESS</Server>
    <Database>medicine_ins</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

FactMedicalEvents.Where(p=>p.FactPatient.AccountId == 2)
.Where(p=>p.DiagnosisGeneralM001.IDDS.Length!=5 && 
M001s.Where(r=>r.IDDS.IndexOf(p.DiagnosisGeneralM001.IDDS) == 0 && r.Payable == 0).Count() > 1)
.Select(p=>p.MedicalEventId)