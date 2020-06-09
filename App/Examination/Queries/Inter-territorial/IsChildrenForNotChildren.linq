<Query Kind="Expression">
  <Connection>
    <ID>b1996b57-fd46-492c-bef8-8d58f766e8f6</ID>
    <Persist>true</Persist>
    <Server>.\SQLEXPRESS</Server>
    <Database>medicine_ins</Database>
    <ShowServer>true</ShowServer>
  </Connection>
</Query>

FactMedicalEvents.Where(p=>p.FactPatient.AccountId == 18).Where(p=>
Math.Floor((p.EventBegin - p.FactPatient.PersonalIdFactPerson.Birthday).Value.Days / 365.25) > 18 && 
(p.IsChildren == true ||
p.ProfileCode.IDPR == 17 ||//детской кардиологии
p.ProfileCode.IDPR == 18 ||//детской онкологии
p.ProfileCode.IDPR == 19 ||//детской урологии-андрологии
p.ProfileCode.IDPR == 20 ||//детской хирургии
p.ProfileCode.IDPR == 21 ||//детской эндокринологии
p.ProfileCode.IDPR == 55 ||//неонатология
p.ProfileCode.IDPR == 68 ||//педиатрия
p.ProfileCode.IDPR == 86 ||//стоматологии детской
p.SpecialityCodeV004.IDMSP == 11 ||//Лечебное дело. Педиатрия
p.SpecialityCodeV004.IDMSP == 1134 ||//Педиатрия
p.SpecialityCodeV004.IDMSP == 2016 ||//Сестринское дело в педиатрии
p.SpecialityCodeV004.IDMSP == 112702 ||//Детская эндокринология
p.SpecialityCodeV004.IDMSP == 112801 ||//Детская онкология
p.SpecialityCodeV004.IDMSP == 113401 ||//Детская онкология
p.SpecialityCodeV004.IDMSP == 113402 ||//Детская эндокринология
p.SpecialityCodeV004.IDMSP == 1135 ||//Детская хирургия
p.SpecialityCodeV004.IDMSP == 113501 ||//Детская онкология
p.SpecialityCodeV004.IDMSP == 113502 ||//Детская урология-андрология
p.SpecialityCodeV004.IDMSP == 140102 //Стоматология детская
)).Select(p=>p.MedicalEventId) 
