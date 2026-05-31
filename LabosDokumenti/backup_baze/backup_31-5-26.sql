/*
SQLyog Community v13.3.0 (64 bit)
MySQL - 8.0.41 : Database - planinarstvodb
*********************************************************************
*/

/*!40101 SET NAMES utf8 */;

/*!40101 SET SQL_MODE=''*/;

/*!40014 SET @OLD_UNIQUE_CHECKS=@@UNIQUE_CHECKS, UNIQUE_CHECKS=0 */;
/*!40014 SET @OLD_FOREIGN_KEY_CHECKS=@@FOREIGN_KEY_CHECKS, FOREIGN_KEY_CHECKS=0 */;
/*!40101 SET @OLD_SQL_MODE=@@SQL_MODE, SQL_MODE='NO_AUTO_VALUE_ON_ZERO' */;
/*!40111 SET @OLD_SQL_NOTES=@@SQL_NOTES, SQL_NOTES=0 */;
CREATE DATABASE /*!32312 IF NOT EXISTS*/`planinarstvodb` /*!40100 DEFAULT CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci */ /*!80016 DEFAULT ENCRYPTION='N' */;

USE `planinarstvodb`;

/*Table structure for table `__efmigrationshistory` */

DROP TABLE IF EXISTS `__efmigrationshistory`;

CREATE TABLE `__efmigrationshistory` (
  `MigrationId` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `ProductVersion` varchar(32) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  PRIMARY KEY (`MigrationId`)
) ENGINE=InnoDB DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

/*Data for the table `__efmigrationshistory` */

insert  into `__efmigrationshistory`(`MigrationId`,`ProductVersion`) values 
('20260507142539_Initial','9.0.0'),
('20260508071058_DodanaObavijest','9.0.0'),
('20260513180902_AddSoftDelete','9.0.0'),
('20260514191620_AddExpandedKontrolneTockeIRute','9.0.0');

/*Table structure for table `fotografije` */

DROP TABLE IF EXISTS `fotografije`;

CREATE TABLE `fotografije` (
  `IdFotografija` int NOT NULL AUTO_INCREMENT,
  `IdPosjet` int NOT NULL,
  `NazivDatoteke` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `PutanjaDatoteke` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `DatumUploada` datetime(6) NOT NULL,
  `TipSlike` int NOT NULL,
  `Opis` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `DeletedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`IdFotografija`),
  KEY `IX_Fotografije_IdPosjet` (`IdPosjet`),
  CONSTRAINT `FK_Fotografije_Posjeti_IdPosjet` FOREIGN KEY (`IdPosjet`) REFERENCES `posjeti` (`IdPosjet`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

/*Data for the table `fotografije` */

insert  into `fotografije`(`IdFotografija`,`IdPosjet`,`NazivDatoteke`,`PutanjaDatoteke`,`DatumUploada`,`TipSlike`,`Opis`,`DeletedAt`) values 
(1,1,'vis_luka_selfie.jpg','/slike/posjeti/vis_luka_selfie.jpg','2026-04-05 12:05:00.000000',0,'Selfie korisnika Luke na vrhu Vis.',NULL),
(2,2,'okic_luka_selfie.jpg','/slike/posjeti/okic_luka_selfie.jpg','2026-04-12 10:05:00.000000',0,'Fotografija Luke kod oznake vrha Okić.',NULL),
(3,3,'japetic_luka_selfie.jpg','/slike/posjeti/japetic_luka_selfie.jpg','2026-04-19 11:15:00.000000',0,'Selfie na vrhu Japetić uz piramidu.',NULL),
(4,4,'vis_test_selfie.jpg','/slike/posjeti/vis_test_selfie.jpg','2026-04-08 12:50:00.000000',0,'Testni korisnik na vrhu Vis.',NULL),
(5,5,'sljeme_test_selfie.jpg','/slike/posjeti/sljeme_test_selfie.jpg','2026-04-26 10:45:00.000000',0,'Testni korisnik na vrhu Sljeme kod oznake.',NULL);

/*Table structure for table `knjizice` */

DROP TABLE IF EXISTS `knjizice`;

CREATE TABLE `knjizice` (
  `IdKnjizica` int NOT NULL AUTO_INCREMENT,
  `IdKorisnik` int NOT NULL,
  `DatumKreiranja` datetime(6) NOT NULL,
  `Napomena` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `StatusAktivna` tinyint(1) NOT NULL,
  PRIMARY KEY (`IdKnjizica`),
  UNIQUE KEY `IX_Knjizice_IdKorisnik` (`IdKorisnik`),
  CONSTRAINT `FK_Knjizice_Korisnici_IdKorisnik` FOREIGN KEY (`IdKorisnik`) REFERENCES `korisnici` (`IdKorisnik`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=5 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

/*Data for the table `knjizice` */

insert  into `knjizice`(`IdKnjizica`,`IdKorisnik`,`DatumKreiranja`,`Napomena`,`StatusAktivna`) values 
(1,1,'2026-04-01 09:05:00.000000','Glavna digitalna knjižica korisnika Luka Bošnjak.',1),
(2,2,'2026-04-01 09:20:00.000000','Testna digitalna knjižica za provjeru funkcionalnosti aplikacije.',1),
(3,3,'2026-05-13 18:30:20.836723',NULL,1),
(4,4,'2026-05-13 18:36:21.411810',NULL,1);

/*Table structure for table `kontrolnetocke` */

DROP TABLE IF EXISTS `kontrolnetocke`;

CREATE TABLE `kontrolnetocke` (
  `IdKontrolnaTocka` int NOT NULL AUTO_INCREMENT,
  `GUIDOznaka` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `IdPodrucje` int NOT NULL,
  `Naziv` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `TipKontrolneTocke` int NOT NULL,
  `NadmorskaVisina` int DEFAULT NULL,
  `Opis` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Koordinate` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `OpisZiga` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `DeletedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`IdKontrolnaTocka`),
  UNIQUE KEY `IX_KontrolneTocke_GUIDOznaka` (`GUIDOznaka`),
  KEY `IX_KontrolneTocke_IdPodrucje` (`IdPodrucje`),
  CONSTRAINT `FK_KontrolneTocke_Podrucja_IdPodrucje` FOREIGN KEY (`IdPodrucje`) REFERENCES `podrucja` (`IdPodrucje`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=62 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

/*Data for the table `kontrolnetocke` */

insert  into `kontrolnetocke`(`IdKontrolnaTocka`,`GUIDOznaka`,`IdPodrucje`,`Naziv`,`TipKontrolneTocke`,`NadmorskaVisina`,`Opis`,`Koordinate`,`OpisZiga`,`DeletedAt`) values 
(1,'MOS1234',2,'Moslavačka gora – vrh Vis',0,437,'Najviši vrh Moslavačke gore i dobra početna kontrolna točka za početničke obilaznike.','N/A','Metalni žig na vršnoj oznaci.',NULL),
(2,'SJE1234',4,'Sljeme – vrh',0,1033,'Najviši vrh Medvednice; vrh je lako dostupan i planinarima i izletnicima.','N 45° 53\' 57.4\'\' E 15° 56\' 50.6\'\'','Metalni žig vrha nalazi se na promidžbenom panou kod televizijskog tornja.',NULL),
(3,'OKI1234',5,'Okić – vrh',0,499,'Stari grad i vršna gradina s vidikom prema Zagrebu i Medvednici.','N 45° 44\' 55.4\'\' E 15° 42\' 24.0\'\'','Metalni žig vrha ugrađen je na zid u najvišem dijelu gradine.',NULL),
(4,'JAP1234',5,'Japetić – vrh',0,879,'Najviši vrh Samoborskoga gorja; poznat po piramidi i domu Žitnica.','N 45° 44\' 56.3\'\' E 15° 36\' 32.8\'\'','Metalni žig ugrađen je na konstrukciju piramide.',NULL),
(5,'VZA1234',11,'Veliki Zavižan – vrh',0,1676,'Jedan od najpoznatijih vrhova Sjevernog Velebita s vrlo atraktivnim pogledima.','N/A','Žig kontrolne točke nalazi se na vrhu ili u blizini planinarskog doma Zavižan.',NULL),
(6,'GRO4920',11,'Gromovača',0,1676,'Najpogodniji i najatraktivniji je prilaz od planinarskog doma Zavižan. Cestom koja vodi kroz Zavižansku kotlinu treba poći još 2 km do putokaza gdje se desno odvaja Premužićeva staza prema Rožanskim kukovima (45\' od planinarskog doma Zavižan). Staza nakon 30\' ulazi u sve krševitije područje i iznenada stiže u kamenito područje Rož','N 44° 46\' 23.3\'\' E 14° 59\' 3.8\'\' ','Na vrhu je ugrađen metalni žig nekoliko metara istočno od vršne točke ',NULL),
(7,'KAP8371',1,'Krndija – vrh Kapovac',0,790,'Najviši vrh Krndije u slavonskom gorju; šumoviti vrh s markiranim pristupom.','N 45° 28\' 12.0\'\' E 17° 52\' 30.0\'\'','Metalni žig na vršnoj oznaci.',NULL),
(8,'IVA5629',1,'Papuk – vrh Ivačka glava',0,913,'Najviši vrh Papuka i cijele Slavonije; dostupan s više strana.','N 45° 31\' 10.0\'\' E 17° 40\' 15.0\'\'','Metalni žig na vrhu kod geodetskog stupa.',NULL),
(9,'BRE7412',1,'Psunj – vrh Brezovo polje',0,984,'Najviši vrh Psunja i jedan od najviših slavonskih vrhova; šumovit i miran.','N 45° 16\' 45.0\'\' E 17° 18\' 20.0\'\'','Metalni žig na oznaci vrha.',NULL),
(10,'STA2087',2,'Bilogora – Stankov vrh',0,309,'Najviši vrh Bilogore s vidikovcem i planinarskim putom kroz šumu.','N 45° 53\' 00.0\'\' E 17° 07\' 30.0\'\'','Metalni žig na drvenom stupu kod vidikovca.',NULL),
(11,'MOH6243',3,'Međimurske gorice – vrh Mohokos',0,344,'Najviši vrh Međimurja; lagani pristup i lijep pogled prema Alpama i Zagorju.','N 46° 24\' 50.0\'\' E 16° 22\' 10.0\'\'','Metalni žig na oznaci vrha.',NULL),
(12,'IVN3815',3,'Ivanščica – vrh Ivanščica',0,1060,'Najviši vrh Hrvatskog zagorja i najistaknutiji zagorski vrh s panoramskim vidicima.','N 46° 10\' 55.0\'\' E 16° 06\' 45.0\'\'','Metalni žig na vršnom stupu.',NULL),
(13,'RAV9174',3,'Ravna gora – vrh (piramida)',0,680,'Šumoviti vrh s geodetskom piramidom i markiranim pristupom iz Gornje Stubice.','N 46° 04\' 20.0\'\' E 15° 56\' 30.0\'\'','Metalni žig na piramidi.',NULL),
(14,'SUS4538',3,'Strahinjščica – vrh Sušec',0,846,'Najviši vrh Strahinjščice s pogledom prema Ivanščici i Krapinskoj dolini.','N 46° 11\' 40.0\'\' E 15° 54\' 20.0\'\'','Metalni žig na kamenoj oznaci vrha.',NULL),
(15,'GRH7260',4,'Grohot – vrh',0,492,'Niži vrh Medvednice s vidikovcem i starim hrastovima; pogodan za kraće ture.','N 45° 52\' 30.0\'\' E 16° 03\' 10.0\'\'','Metalni žig na drvenoj oznaci vrha.',NULL),
(16,'LIP3492',4,'Lipa – vrh',0,709,'Šumoviti vrh Medvednice na sjevernom grebenu; miran i manje posjećen.','N 45° 54\' 10.0\'\' E 15° 55\' 40.0\'\'','Metalni žig na vršnom stupu.',NULL),
(17,'MEG8156',4,'Medvedgrad',2,579,'Srednjovjekovna utvrda na južnim padinama Medvednice; kontrolna točka HPO-a.','N 45° 51\' 45.0\'\' E 15° 56\' 50.0\'\'','Metalni žig na ulaznom zidu utvrde.',NULL),
(18,'PLE6703',5,'Plešivica – vrh',0,779,'Vrh Samoborskog gorja s pogledom na vinograde i Žumberak; blizu planinarskog doma.','N 45° 43\' 30.0\'\' E 15° 39\' 20.0\'\'','Metalni žig na kamenoj oznaci.',NULL),
(19,'OST5281',5,'Oštrc – vrh',0,752,'Popularan vrh s kapelom Sv. Ane na vrhu i panoramskim vidicima.','N 45° 44\' 10.0\'\' E 15° 40\' 55.0\'\'','Metalni žig na kapelici na vrhu.',NULL),
(20,'TUS9047',6,'Tuščak – gradina',2,585,'Stara gradina na zapadnom dijelu Žumberačke gore; pogled prema Žumberku.','N 45° 44\' 00.0\'\' E 15° 30\' 10.0\'\'','Metalni žig na ruševini gradine.',NULL),
(21,'SGE2634',6,'Sveta Gera – vrh',0,1178,'Najviši vrh Žumberačke gore i cijele Žumberačko-samoborske regije.','N 45° 42\' 45.0\'\' E 15° 22\' 30.0\'\'','Metalni žig na vršnom stupu.',NULL),
(22,'PLI7819',6,'Pliješ – vrh',0,977,'Šumoviti vrh Žumberačke gore s markiranim putom iz Budinjaka.','N 45° 43\' 20.0\'\' E 15° 25\' 50.0\'\'','Metalni žig na oznaci vrha.',NULL),
(23,'VOD4153',7,'Vodenica – vrh',0,538,'Najviši vrh Pokuplja; miran vrh s pogledom na Kupu i okolne šume.','N 45° 27\' 10.0\'\' E 15° 32\' 20.0\'\'','Metalni žig na drvenoj oznaci.',NULL),
(24,'PET6928',7,'Petrova gora – vrh Petrovac',0,512,'Vrh Petrove gore s poznatim spomenikom i vidikovcem prema Kordunu.','N 45° 19\' 20.0\'\' E 15° 47\' 00.0\'\'','Metalni žig na spomeniku kod vrha.',NULL),
(25,'KLE3047',8,'Klek – vrh',0,1181,'Karakteristična stijena iznad Ogulina; simbol hrvatskog planinarstva od 1874. godine.','N 45° 17\' 55.0\'\' E 15° 10\' 40.0\'\'','Metalni žig na vršnom stupu.',NULL),
(26,'BJE8592',8,'Bjelolasica – vrh Kula',0,1534,'Najviši vrh Gorskog kotara i hrvatski vrh izvan Velebita i Dinare.','N 45° 15\' 50.0\'\' E 14° 58\' 30.0\'\'','Metalni žig na geodetskom stupu na vrhu.',NULL),
(27,'SAM1736',8,'Samarske stijene – vrh',0,1302,'Spektakularne stjenovite formacije u srcu Gorskog kotara; zahtjevan pristup.','N 45° 16\' 20.0\'\' E 14° 55\' 10.0\'\'','Metalni žig na stijeni kod vrha.',NULL),
(28,'RIS4208',9,'Risnjak – vrh',0,1528,'Najviši vrh istoimenog nacionalnog parka; panoramski pogled od Alpa do mora.','N 45° 25\' 35.0\'\' E 14° 45\' 20.0\'\'','Metalni žig na vršnom stupu kod kapelice.',NULL),
(29,'SNJ6371',9,'Snježnik – vrh',0,1505,'Drugi najviši vrh Gorskog kotara; poznat po kasnom snijegu i alpskim livadama.','N 45° 26\' 10.0\'\' E 14° 35\' 40.0\'\'','Metalni žig na kamenoj oznaci vrha.',NULL),
(30,'SKR2845',9,'Skradski vrh',0,1043,'Popularan izletnički vrh u sjevernom Gorskom kotaru s planinarskim domom.','N 45° 24\' 05.0\'\' E 15° 02\' 15.0\'\'','Metalni žig na vršnom stupu.',NULL),
(31,'VOJ7164',10,'Učka – vrh Vojak',0,1396,'Najviši vrh Istre s kamenim tornjem na vrhu i pogledom na Kvarner i Alpe.','N 45° 17\' 10.0\'\' E 14° 11\' 55.0\'\'','Metalni žig na kamenom tornju na vrhu.',NULL),
(32,'VPL3920',10,'Ćićarija – vrh Veliki Planik',0,1272,'Najviši vrh Ćićarije s travnatim vršnim područjem i pogledom prema Učki.','N 45° 27\' 20.0\'\' E 14° 13\' 30.0\'\'','Metalni žig na kamenoj oznaci.',NULL),
(33,'MRA8451',11,'Mali Rajinac – vrh',0,1699,'Jedan od najviših velebitskih vrhova na sjevernom dijelu; krški vrh s divljim pogledom.','N 44° 46\' 30.0\'\' E 14° 58\' 50.0\'\'','Metalni žig na vrhu stijene.',NULL),
(34,'ZEC6237',12,'Zečjak – vrh',0,1622,'Najviši vrh Srednjeg Velebita; stjenovit i zahtjevan teren.','N 44° 36\' 15.0\'\' E 15° 03\' 40.0\'\'','Metalni žig na kamenoj piramidi.',NULL),
(35,'SAT1584',12,'Šatorina – vrh',0,1622,'Karakteristični vrh Srednjeg Velebita s oblikom šatora; divlji krški krajolik.','N 44° 34\' 50.0\'\' E 15° 05\' 10.0\'\'','Metalni žig na oznaci vrha.',NULL),
(36,'VAG7302',13,'Vaganski vrh',0,1757,'Najviši vrh Velebita i treći najviši vrh Hrvatske; zahtjevan pristup iz Paklenice.','N 44° 21\' 50.0\'\' E 15° 30\' 20.0\'\'','Metalni žig na geodetskom stupu na vrhu.',NULL),
(37,'SVB4916',13,'Sveto brdo – vrh',0,1751,'Drugi najviši vrh Velebita s kapelom na vrhu; pogled na more i Liku.','N 44° 19\' 40.0\'\' E 15° 30\' 55.0\'\'','Metalni žig na kapeli na vrhu.',NULL),
(38,'ANI2058',13,'Anića kuk – vrh',0,712,'Impozantna stijena u klancu Velike Paklenice; alpinistički značajan vrh.','N 44° 18\' 15.0\'\' E 15° 27\' 40.0\'\'','Metalni žig na vršnom stupu.',NULL),
(39,'OZE8743',14,'Lička Plješivica – vrh Ozeblin',0,1657,'Najviši vrh Ličke Plješivice i Like; zahtjevan pristup šumskim putovima.','N 44° 46\' 10.0\'\' E 15° 44\' 30.0\'\'','Metalni žig na vrhu.',NULL),
(40,'POT5261',14,'Poštak – vrh',0,1425,'Istaknuti lički vrh na granici prema Dalmaciji s otvorenim pogledom.','N 44° 10\' 55.0\'\' E 16° 10\' 20.0\'\'','Metalni žig na vršnoj oznaci.',NULL),
(41,'OBZ3179',15,'Krk – vrh Obzova',0,569,'Najviši vrh otoka Krka s pogledom na Kvarner i okolne otoke.','N 45° 01\' 20.0\'\' E 14° 37\' 50.0\'\'','Metalni žig na vršnom stupu.',NULL),
(42,'SIS6420',15,'Cres – vrh Sis',0,639,'Najviši vrh otoka Cresa; divlji otočni krajolik s pogledom na Jadran.','N 44° 52\' 30.0\'\' E 14° 22\' 10.0\'\'','Metalni žig na kamenoj oznaci.',NULL),
(43,'VID8537',16,'Brač – vrh Vidova gora',0,780,'Najviši vrh jadranskih otoka; spektakularan pogled na Zlatni rat i Hvar.','N 43° 18\' 40.0\'\' E 16° 37\' 20.0\'\'','Metalni žig na vršnom stupu.',NULL),
(44,'SNK2074',16,'Hvar – vrh Sv. Nikola',0,626,'Najviši vrh otoka Hvara s pogledom na paklinske otoke i pelješku obalu.','N 43° 10\' 35.0\'\' E 16° 39\' 50.0\'\'','Metalni žig na kapelici Sv. Nikole.',NULL),
(45,'KOM9361',16,'Korčula – vrh Kom',0,508,'Najviši vrh otoka Korčule s gustim makijama i pogledom na Pelješac.','N 42° 57\' 30.0\'\' E 16° 53\' 15.0\'\'','Metalni žig na kamenoj oznaci.',NULL),
(46,'DIN4728',17,'Dinara – vrh Dinara (Sinjal)',0,1831,'Najviši vrh Republike Hrvatske; obavezna kontrolna točka za srebrnu značku HPO-a.','N 43° 59\' 25.0\'\' E 16° 22\' 50.0\'\'','Metalni žig na geodetskom stupu na vrhu.',NULL),
(47,'SVL5839',17,'Svilaja – vrh Svilaja',0,1508,'Najviši vrh planine Svilaje u dalmatinskom zaleđu; zahtjevan pristup.','N 43° 44\' 10.0\'\' E 16° 28\' 30.0\'\'','Metalni žig na vršnom stupu.',NULL),
(48,'CAV7162',17,'Promina – vrh Čavnovka',0,1147,'Najviši vrh planine Promine iznad Drniša; pogled na Krku i Zagoru.','N 43° 51\' 40.0\'\' E 16° 05\' 20.0\'\'','Metalni žig na kamenoj oznaci.',NULL),
(49,'LJU3084',18,'Mosor – vrh Ljubljan',0,1262,'Istaknuti vrh Mosora iznad Splita; markiran pristup iz Dugopolja.','N 43° 31\' 20.0\'\' E 16° 31\' 50.0\'\'','Metalni žig na vrhu.',NULL),
(50,'BIR6597',18,'Kozjak – vrh Biranj',0,631,'Vrh planine Kozjak iznad Kaštela s pogledom na Split i otoke.','N 43° 33\' 50.0\'\' E 16° 24\' 10.0\'\'','Metalni žig na kamenoj oznaci.',NULL),
(51,'SJU4213',19,'Sv. Jure – vrh',0,1762,'Najviši vrh Biokova i drugi najviši vrh uz obalu; pristup cestom ili pješice.','N 43° 20\' 10.0\'\' E 17° 03\' 00.0\'\'','Metalni žig na kapeli Sv. Jure.',NULL),
(52,'VOS8746',19,'Vošac – vrh',0,1421,'Popularan biokovački vrh s pogledom na makarsku rivijeru i otoke.','N 43° 18\' 55.0\'\' E 17° 04\' 20.0\'\'','Metalni žig na vršnom stupu.',NULL),
(53,'KIM3509',19,'Kimet – vrh',0,1536,'Zahtjevniji biokovački vrh; stjenovit i izložen vjetru.','N 43° 19\' 30.0\'\' E 17° 04\' 50.0\'\'','Metalni žig na stijeni.',NULL),
(54,'SIL2871',20,'Pelješac – vrh Sv. Ilija',0,960,'Najviši vrh poluotoka Pelješca; zahtjevna staza s pogledom na Korčulu i Mljet.','N 42° 55\' 20.0\'\' E 17° 07\' 30.0\'\'','Metalni žig na vršnom stupu.',NULL),
(55,'ILJ6034',20,'Sniježnica – Ilijin vrh',0,1234,'Najviši vrh dubrovačkog zaleđa; panoramski pogled od Dubrovnika do crnogorskih planina.','N 42° 38\' 40.0\'\' E 18° 15\' 10.0\'\'','Metalni žig na vrhu.',NULL),
(56,'VSV7283',8,'Viševica – vrh',0,1428,'Istaknuti vrh južnog Gorskog kotara s pogledom na Kvarner i otoke.','N 45° 18\' 40.0\'\' E 14° 39\' 50.0\'\'','Metalni žig na vršnom stupu.',NULL),
(57,'CAR5190',1,'Dilj gora – vrh Čardak',0,421,'Najviši vrh Dilj gore kod Slavonskog Broda; blag i pristupačan vrh.','N 45° 14\' 30.0\'\' E 18° 07\' 20.0\'\'','Metalni žig na vršnoj oznaci.',NULL),
(58,'ZBE8416',10,'Ćićarija – vrh Žbevnica',0,1014,'Vrh Ćićarije s travnatim vršnim područjem i pogledom prema slovenskoj granici.','N 45° 29\' 10.0\'\' E 14° 08\' 40.0\'\'','Metalni žig na kamenoj oznaci.',NULL),
(59,'VRA2758',3,'Kalnik – vrh Vranilac',0,643,'Najviši vrh Kalnika sa stijenama i pogledom na Podravinu; zahtjevniji pristup.','N 46° 09\' 00.0\'\' E 16° 27\' 30.0\'\'','Metalni žig na stijeni kod vrha.',NULL),
(60,'HOR6391',4,'Horvatovih 500 stuba',2,450,'Poznate stube na Medvednici; jedna od dvije kontrolne točke HPO-a koje nisu vrhovi.','N 45° 52\' 10.0\'\' E 15° 57\' 20.0\'\'','Metalni žig na oznaci kod stuba.',NULL),
(61,'ANI2057',13,'Anića kuk – vrh',0,NULL,NULL,NULL,NULL,'2026-05-31 15:32:31.009454');

/*Table structure for table `korisnici` */

DROP TABLE IF EXISTS `korisnici`;

CREATE TABLE `korisnici` (
  `IdKorisnik` int NOT NULL AUTO_INCREMENT,
  `Ime` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Prezime` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Email` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `KorisnickoIme` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `PasswordHash` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `DatumRodenja` datetime(6) DEFAULT NULL,
  `DatumRegistracije` datetime(6) NOT NULL,
  `BrojMobitela` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ProfilnaSlika` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `StatusAktivan` tinyint(1) NOT NULL,
  PRIMARY KEY (`IdKorisnik`),
  UNIQUE KEY `IX_Korisnici_Email` (`Email`),
  UNIQUE KEY `IX_Korisnici_KorisnickoIme` (`KorisnickoIme`)
) ENGINE=InnoDB AUTO_INCREMENT=8 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

/*Data for the table `korisnici` */

insert  into `korisnici`(`IdKorisnik`,`Ime`,`Prezime`,`Email`,`KorisnickoIme`,`PasswordHash`,`DatumRodenja`,`DatumRegistracije`,`BrojMobitela`,`ProfilnaSlika`,`StatusAktivan`) values 
(1,'Luka','Bošnjak','luka.bosnjak92@gmail.com','Boss','123456789','2004-06-29 00:00:00.000000','2026-04-01 09:00:00.000000','0979545897','/Slike/Profil/Boss.jpeg',1),
(2,'Test','Test','test123@gmail.com','Test','123456789','2005-01-01 00:00:00.000000','2026-04-01 09:15:00.000000',NULL,'/Slike/Profil/test.jpg',1),
(3,'Paula','Šoštarić','paula.sostaric03@gmail.com','Paula','ChangeMe123!',NULL,'2026-05-13 18:30:01.806835','0994537998',NULL,1),
(4,'Siniša','Šoštarić','DrSinisaSostaric@hotmail.com','Sinke','ChangeMe123!','1972-03-13 00:00:00.000000','2026-05-13 18:36:09.722938','0979554828',NULL,1),
(5,'Borna','Horina','bhorina@tvz.hr','GeneralBorz','ChangeMe123!','2004-05-05 00:00:00.000000','2026-05-14 17:31:27.215160','0998385621',NULL,0),
(6,'Ilija','Bosnjak','ibosnjak@gmail.com','Zenka','ChangeMe123!','2004-05-19 22:00:00.000000','2026-05-14 18:01:20.078889',NULL,NULL,0);

/*Table structure for table `korisnikmedalje` */

DROP TABLE IF EXISTS `korisnikmedalje`;

CREATE TABLE `korisnikmedalje` (
  `IdKorisnikMedalja` int NOT NULL AUTO_INCREMENT,
  `IdKorisnik` int NOT NULL,
  `IdMedalja` int NOT NULL,
  `DatumDodjele` datetime(6) NOT NULL,
  `Napomena` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `DeletedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`IdKorisnikMedalja`),
  KEY `IX_KorisnikMedalje_IdKorisnik` (`IdKorisnik`),
  KEY `IX_KorisnikMedalje_IdMedalja` (`IdMedalja`),
  CONSTRAINT `FK_KorisnikMedalje_Korisnici_IdKorisnik` FOREIGN KEY (`IdKorisnik`) REFERENCES `korisnici` (`IdKorisnik`) ON DELETE CASCADE,
  CONSTRAINT `FK_KorisnikMedalje_Medalje_IdMedalja` FOREIGN KEY (`IdMedalja`) REFERENCES `medalje` (`IdMedalja`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

/*Data for the table `korisnikmedalje` */

insert  into `korisnikmedalje`(`IdKorisnikMedalja`,`IdKorisnik`,`IdMedalja`,`DatumDodjele`,`Napomena`,`DeletedAt`) values 
(1,1,1,'2026-04-19 12:00:00.000000','Korisnik je zadovoljio uvjet početničke medalje jer ima evidentiran obilazak područja 2 (Moslavačka gora i Bilogora), gdje je prag 1 KT.',NULL),
(2,2,1,'2026-04-08 13:00:00.000000','Korisnik je zadovoljio uvjet početničke medalje jer ima evidentiran obilazak područja 2 (Moslavačka gora i Bilogora), gdje je prag 1 KT.',NULL),
(3,3,1,'2026-05-14 18:23:27.918003','Automatski dodijeljeno kroz eligibility stranicu.','2026-05-14 18:23:49.291828'),
(4,4,1,'2026-05-14 18:23:37.161537','Automatski dodijeljeno kroz eligibility stranicu.','2026-05-14 18:23:45.123256'),
(5,3,1,'2026-05-14 18:32:08.913903','Automatski dodijeljeno kroz eligibility stranicu.',NULL),
(6,4,1,'2026-05-15 11:37:28.556596','Automatski dodijeljeno kroz eligibility stranicu.',NULL);

/*Table structure for table `medalje` */

DROP TABLE IF EXISTS `medalje`;

CREATE TABLE `medalje` (
  `IdMedalja` int NOT NULL AUTO_INCREMENT,
  `Naziv` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Opis` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `MinimalanBrojKontrolnihTocaka` int NOT NULL,
  `MinimalanBrojPodrucja` int NOT NULL,
  `DeletedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`IdMedalja`)
) ENGINE=InnoDB AUTO_INCREMENT=9 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

/*Data for the table `medalje` */

insert  into `medalje`(`IdMedalja`,`Naziv`,`Opis`,`MinimalanBrojKontrolnihTocaka`,`MinimalanBrojPodrucja`,`DeletedAt`) values 
(1,'Početnik','Osnovna medalja za prvi ispravno evidentirani obilazak područja.',1,1,'2026-05-31 15:07:10.541233'),
(2,'Brončana značka','Potrebno je obići zadani broj KT-a iz 10 područja i ukupno 25 KT-a.',25,10,NULL),
(3,'Srebrna značka','Potrebno je obići zadani broj KT-a iz 15 područja i ukupno 50 KT-a, uz obaveznu Dinaru (Sinjal).',50,15,NULL),
(4,'Zlatna značka','Potrebno je obići zadani broj KT-a iz svih 20 područja i ukupno 75 KT-a.',75,20,NULL),
(5,'Posebno priznanje','Potrebno je obići 100 KT-a uz ispunjene uvjete za zlatnu značku.',100,20,NULL),
(6,'Visoko priznanje','Potrebno je obići 125 KT-a uz ispunjene uvjete za posebno priznanje.',125,20,NULL),
(7,'Najviše priznanje','Potrebno je obići 155 KT-a uz ispunjene uvjete za visoko priznanje.',155,20,NULL),
(8,'Početnik',NULL,1,1,NULL);

/*Table structure for table `obavijesti` */

DROP TABLE IF EXISTS `obavijesti`;

CREATE TABLE `obavijesti` (
  `IdObavijest` int NOT NULL AUTO_INCREMENT,
  `Naslov` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Sadrzaj` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `DatumObjave` datetime(6) NOT NULL,
  `JeAktivna` tinyint(1) NOT NULL,
  `IdKorisnik` int NOT NULL,
  PRIMARY KEY (`IdObavijest`),
  KEY `IX_Obavijesti_IdKorisnik` (`IdKorisnik`),
  CONSTRAINT `FK_Obavijesti_Korisnici_IdKorisnik` FOREIGN KEY (`IdKorisnik`) REFERENCES `korisnici` (`IdKorisnik`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=4 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

/*Data for the table `obavijesti` */

insert  into `obavijesti`(`IdObavijest`,`Naslov`,`Sadrzaj`,`DatumObjave`,`JeAktivna`,`IdKorisnik`) values 
(1,'Dobrodošli u planinarsku aplikaciju','Aplikacija je pokrenuta i spremna za korištenje.','2026-04-01 10:00:00.000000',1,1),
(2,'Nova ruta dodana: Zavižan','Dodana je kružna tura od doma Zavižan preko Balinovca.','2026-04-15 14:30:00.000000',1,1);

/*Table structure for table `planinarskeudruge` */

DROP TABLE IF EXISTS `planinarskeudruge`;

CREATE TABLE `planinarskeudruge` (
  `IdPlaninarskaUdruga` int NOT NULL AUTO_INCREMENT,
  `OIB` varchar(11) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Naziv` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Email` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `BrojTelefona` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Adresa` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `PostanskiBroj` varchar(20) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Grad` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Zupanija` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `BrojClanova` int DEFAULT NULL,
  `DeletedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`IdPlaninarskaUdruga`),
  UNIQUE KEY `IX_PlaninarskeUdruge_OIB` (`OIB`)
) ENGINE=InnoDB AUTO_INCREMENT=6 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

/*Data for the table `planinarskeudruge` */

insert  into `planinarskeudruge`(`IdPlaninarskaUdruga`,`OIB`,`Naziv`,`Email`,`BrojTelefona`,`Adresa`,`PostanskiBroj`,`Grad`,`Zupanija`,`BrojClanova`,`DeletedAt`) values 
(1,'40461293872','HPD Mosor','hpd.mosor@hps.hr',NULL,'p.p. 233','21000','Split','Splitsko-dalmatinska',350,NULL),
(2,'48938096579','HPD Gora','hpd.gora@hps.hr',NULL,'Dubravica 27a','10000','Zagreb','Grad Zagreb',180,NULL),
(3,'95873199484','PD Zavižan','pd.zavizan@hps.hr',NULL,'Mala vrata 20','53270','Senj','Ličko-senjska',120,NULL),
(4,'92966614510','PD Paklenica','pd.paklenica@hps.hr',NULL,'Majke Margarite 6','23000','Zadar','Zadarska',220,NULL),
(5,'12345678901','PD Dr. Maks Plotnikov','info@pddr-maks-plotnikov.hr','0991234567','Andrije Hebranga 26','10430','Samobor','Zagrebačka',95,NULL);

/*Table structure for table `planinarskiobjekti` */

DROP TABLE IF EXISTS `planinarskiobjekti`;

CREATE TABLE `planinarskiobjekti` (
  `IdPlaninarskiObjekt` int NOT NULL AUTO_INCREMENT,
  `IdPodrucje` int NOT NULL,
  `IdPlaninarskaUdruga` int NOT NULL,
  `Naziv` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `TipObjekta` int NOT NULL,
  `NadmorskaVisina` int DEFAULT NULL,
  `Kapacitet` int DEFAULT NULL,
  `Opis` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `ImeOdgovorneOsobe` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Telefon` varchar(30) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Email` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `Adresa` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `ImaNocenje` tinyint(1) NOT NULL,
  `ImaHranu` tinyint(1) NOT NULL,
  `RadnoVrijemeOpis` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `DeletedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`IdPlaninarskiObjekt`),
  KEY `IX_PlaninarskiObjekti_IdPlaninarskaUdruga` (`IdPlaninarskaUdruga`),
  KEY `IX_PlaninarskiObjekti_IdPodrucje` (`IdPodrucje`),
  CONSTRAINT `FK_PlaninarskiObjekti_PlaninarskeUdruge_IdPlaninarskaUdruga` FOREIGN KEY (`IdPlaninarskaUdruga`) REFERENCES `planinarskeudruge` (`IdPlaninarskaUdruga`) ON DELETE CASCADE,
  CONSTRAINT `FK_PlaninarskiObjekti_Podrucja_IdPodrucje` FOREIGN KEY (`IdPodrucje`) REFERENCES `podrucja` (`IdPodrucje`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=7 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

/*Data for the table `planinarskiobjekti` */

insert  into `planinarskiobjekti`(`IdPlaninarskiObjekt`,`IdPodrucje`,`IdPlaninarskaUdruga`,`Naziv`,`TipObjekta`,`NadmorskaVisina`,`Kapacitet`,`Opis`,`ImeOdgovorneOsobe`,`Telefon`,`Email`,`Adresa`,`ImaNocenje`,`ImaHranu`,`RadnoVrijemeOpis`,`DeletedAt`) values 
(1,5,5,'Planinarski dom Dr. Maks Plotnikov',0,411,14,'Dom podno ruševina Okić-grada; polazišna je točka za Okić i okolne putove.','Stjepan Jandrečić','0918909624','aplantosar@gmail.com','Okić, Samobor',1,1,'Otvoren vikendom i blagdanima.',NULL),
(2,5,2,'Planinarski dom Željezničar',0,691,25,'Popularan planinarski dom u Samoborskom i Žumberačkom gorju.','Dežurni domar',NULL,NULL,'Samoborsko gorje',1,1,'Prema rasporedu dežurstva i vikendom.',NULL),
(3,11,3,'Planinarska kuća Sijaset',1,328,12,'Niži planinarski objekt na Velebitu, pogodan kao polazište za ture.','Dežurni član društva',NULL,'pd.zavizan@hps.hr','Velebit, Senj',1,0,'Povremeno otvorena ili po dogovoru.',NULL),
(4,13,4,'Planinarski dom Paklenica',0,480,44,'Dom na početku klanca Velike Paklenice s hranom, pićem i noćenjem.','Irena Šaran','0977557654','pd.paklenica@hps.hr','Velika Paklenica',1,1,'Otvoren stalno.',NULL),
(5,18,1,'Planinarska kuća Lugarnica',1,872,20,'Planinarska kuća na Mosoru pogodna za kraće i srednje duge uspone.','Dežurna osoba društva',NULL,'hpd.mosor@hps.hr','Mosor, Split',1,0,'Otvorenost prema obavijesti društva.',NULL);

/*Table structure for table `podrucja` */

DROP TABLE IF EXISTS `podrucja`;

CREATE TABLE `podrucja` (
  `IdPodrucje` int NOT NULL AUTO_INCREMENT,
  `Naziv` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Opis` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `Regija` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `MinimalanBrojKTZaObilazak` int NOT NULL,
  `DeletedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`IdPodrucje`)
) ENGINE=InnoDB AUTO_INCREMENT=24 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

/*Data for the table `podrucja` */

insert  into `podrucja`(`IdPodrucje`,`Naziv`,`Opis`,`Regija`,`MinimalanBrojKTZaObilazak`,`DeletedAt`) values 
(1,'Slavonija','Nizinsko i brežuljkasto područje istočne Hrvatske s Papukom, Psunjem, Krndijom i drugim slavonskim gorjima.','Istočna Hrvatska',2,NULL),
(2,'Moslavačka gora i Bilogora','Niža šumovita gorja s kraćim planinarskim usponima i manjim brojem kontrolnih točaka.','Središnja Hrvatska',1,NULL),
(3,'Hrvatsko zagorje i Međimurje','Brežuljkasto područje s vidikovcima, utvrdama i poznatim vrhovima kao što su Ivanščica i Ravna gora.','Sjeverna Hrvatska',3,NULL),
(4,'Medvednica','Planina iznad Zagreba s gusto razvijenom mrežom putova, domova i kontrolnih točaka.','Središnja Hrvatska',2,NULL),
(5,'Samoborsko gorje','Popularno planinarsko područje zapadno od Zagreba, poznato po Okiću, Japetiću i Oštrcu.','Središnja Hrvatska',2,NULL),
(6,'Žumberačka gora','Planinsko i granično područje s višim vrhovima i rjeđe naseljenim grebenima.','Središnja Hrvatska',1,NULL),
(7,'Karlovačko pokuplje, Kordun i Banovina','Područje nižih gora i šumovitih uzvisina južno od Karlovca i prema Banovini.','Središnja Hrvatska',1,NULL),
(8,'Gorski kotar - južni dio','Dio Gorskog kotara s višim vrhovima, stjenovitim skupinama i zahtjevnijim usponima.','Gorska Hrvatska',4,NULL),
(9,'Gorski kotar - sjeverni dio','Šumovito i planinsko područje s vrhovima poput Risnjaka, Snježnika i Skradskog vrha.','Gorska Hrvatska',3,NULL),
(10,'Istra','Područje Učke i Ćićarije s istaknutim obalnim i planinskim vidikovcima.','Zapadna Hrvatska',2,NULL),
(11,'Sjeverni Velebit','Visokoplaninsko područje s izrazito atraktivnim velebitskim vrhovima i oštrim kršem.','Primorsko-gorska Hrvatska',3,NULL),
(12,'Srednji Velebit','Središnji dio Velebita sa srednje zahtjevnim i zahtjevnim vrhovima i planinarskim kućama.','Lika i Primorje',2,NULL),
(13,'Južni Velebit','Najviši i alpinistički najdojmljiviji dio Velebita s Vaganskim vrhom i Svetim brdom.','Lika i Dalmacija',3,NULL),
(14,'Lika','Prostrano područje ličkih planina i osamljenih vrhova izvan glavnog velebitskog lanca.','Lika',1,NULL),
(15,'Jadranski otoci - sjeverni dio','Sjeverni jadranski otoci s nižim, ali vrlo atraktivnim otočnim vrhovima.','Jadranska Hrvatska',1,NULL),
(16,'Jadranski otoci - južni dio','Južni jadranski otoci s većim brojem otočnih vrhova i raznolikim podlogama.','Jadranska Hrvatska',2,NULL),
(17,'Dalmatinska zagora','Područje Dinare, Promine, Svilaje i drugih planina dalmatinskog zaleđa.','Dalmatinsko zaleđe',2,NULL),
(18,'Dalmacija','Priobalno i zaleđno područje srednje Dalmacije s planinama uz obalu i u zaleđu.','Dalmacija',2,NULL),
(19,'Biokovo i Zagora','Krševito visokoplaninsko područje Biokova i zaleđa s vrlo izraženim visinskim razlikama.','Južna Dalmacija',3,NULL),
(20,'Dubrovačko područje','Južnohrvatsko područje s manjim brojem, ali vrlo atraktivnih kontrolnih točaka.','Krajnji jug Hrvatske',1,NULL),
(21,'Testno područje',NULL,'Slavonija',1,'2026-05-14 17:34:34.643307'),
(22,'Slavonija',NULL,NULL,2,'2026-05-31 15:16:02.393712'),
(23,'Slavonija',NULL,'Slavonija',2,'2026-05-31 15:27:13.503298');

/*Table structure for table `posjeti` */

DROP TABLE IF EXISTS `posjeti`;

CREATE TABLE `posjeti` (
  `IdPosjet` int NOT NULL AUTO_INCREMENT,
  `IdKorisnik` int NOT NULL,
  `IdKnjizica` int NOT NULL,
  `IdKontrolnaTocka` int NOT NULL,
  `IdRuta` int NOT NULL,
  `DatumVrijemePosjeta` datetime(6) NOT NULL,
  `VrijemeUsponaMin` int DEFAULT NULL,
  `DozivljajPosjeta` int NOT NULL,
  `OpisIskustva` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `UneseniGUID` varchar(100) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `JeLiPotvrdenPosjet` tinyint(1) NOT NULL,
  `DatumKreiranjaZapisa` datetime(6) NOT NULL,
  `DeletedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`IdPosjet`),
  KEY `IX_Posjeti_IdKnjizica` (`IdKnjizica`),
  KEY `IX_Posjeti_IdKontrolnaTocka` (`IdKontrolnaTocka`),
  KEY `IX_Posjeti_IdKorisnik` (`IdKorisnik`),
  KEY `IX_Posjeti_IdRuta` (`IdRuta`),
  CONSTRAINT `FK_Posjeti_Knjizice_IdKnjizica` FOREIGN KEY (`IdKnjizica`) REFERENCES `knjizice` (`IdKnjizica`) ON DELETE CASCADE,
  CONSTRAINT `FK_Posjeti_KontrolneTocke_IdKontrolnaTocka` FOREIGN KEY (`IdKontrolnaTocka`) REFERENCES `kontrolnetocke` (`IdKontrolnaTocka`) ON DELETE CASCADE,
  CONSTRAINT `FK_Posjeti_Korisnici_IdKorisnik` FOREIGN KEY (`IdKorisnik`) REFERENCES `korisnici` (`IdKorisnik`) ON DELETE CASCADE,
  CONSTRAINT `FK_Posjeti_Rute_IdRuta` FOREIGN KEY (`IdRuta`) REFERENCES `rute` (`IdRuta`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=14 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

/*Data for the table `posjeti` */

insert  into `posjeti`(`IdPosjet`,`IdKorisnik`,`IdKnjizica`,`IdKontrolnaTocka`,`IdRuta`,`DatumVrijemePosjeta`,`VrijemeUsponaMin`,`DozivljajPosjeta`,`OpisIskustva`,`UneseniGUID`,`JeLiPotvrdenPosjet`,`DatumKreiranjaZapisa`,`DeletedAt`) values 
(1,1,1,1,1,'2026-04-05 10:15:00.000000',92,0,'Prvi evidentirani uspon u aplikaciji. Lagana i ugodna ruta po suhom vremenu.','KT-HPO-2-1-VIS',1,'2026-04-05 12:00:00.000000',NULL),
(2,1,1,3,3,'2026-04-12 08:40:00.000000',43,5,'Kratak, ali strm završni dio prema gradini Okić.','KT-HPO-5-1-OKIC',1,'2026-04-12 10:00:00.000000',NULL),
(3,1,1,4,4,'2026-04-19 09:10:00.000000',96,2,'Ugodna tura s dobrim vremenom i lijepim pogledima s piramide.','KT-HPO-5-4-JAPETIC',1,'2026-04-19 11:10:00.000000',NULL),
(4,2,2,1,1,'2026-04-08 11:00:00.000000',95,1,'Testni korisnik uspješno evidentirao svoj prvi posjet i time zadovoljio uvjet za početničku medalju.','KT-HPO-2-1-VIS',1,'2026-04-08 12:45:00.000000',NULL),
(5,2,2,2,2,'2026-04-26 07:50:00.000000',155,7,'Duži uspon do Sljemena preko Puntijarke, ali bez tehnički teških dijelova.','KT-HPO-4-4-SLJEME',1,'2026-04-26 10:40:00.000000',NULL),
(6,4,4,4,4,'2026-05-14 06:04:00.000000',20,1,NULL,'KT-HPO-5-4-JAPETIC',0,'2026-05-14 04:05:41.350586',NULL),
(7,1,1,2,2,'2026-05-14 06:07:00.000000',NULL,0,NULL,'KT-HPO-5-1-OKIC',0,'2026-05-14 04:08:50.805530',NULL),
(8,1,1,2,2,'2026-05-14 19:46:00.000000',89,1,NULL,'KT-HPO-4-4-SLJEME',0,'2026-05-14 17:47:36.431389',NULL),
(9,3,3,2,2,'2026-05-14 19:51:00.000000',NULL,2,NULL,'KT-HPO-4-4-SLJEME',0,'2026-05-14 17:51:33.050605',NULL),
(10,4,4,2,2,'2026-05-09 10:00:00.000000',50,0,NULL,'KT-HPO-4-4-SLJEME',0,'2026-05-14 17:59:50.953853',NULL),
(11,3,3,3,3,'2026-05-16 09:00:00.000000',85,0,NULL,'OKI1234',0,'2026-05-14 18:16:29.976581',NULL),
(12,1,1,2,2,'2026-05-21 10:00:00.000000',NULL,0,NULL,'SJE1234',0,'2026-05-15 11:39:21.636735',NULL),
(13,1,1,2,2,'2026-05-15 10:00:00.000000',NULL,0,NULL,'SJE1234',0,'2026-05-15 11:40:59.517641',NULL);

/*Table structure for table `rute` */

DROP TABLE IF EXISTS `rute`;

CREATE TABLE `rute` (
  `IdRuta` int NOT NULL AUTO_INCREMENT,
  `IdKontrolnaTocka` int NOT NULL,
  `Naziv` varchar(200) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Pocetak` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `Kraj` varchar(150) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci NOT NULL,
  `VrijemeHodaMin` int NOT NULL,
  `DuljinaKm` decimal(5,2) NOT NULL,
  `VisinskaRazlikaM` int DEFAULT NULL,
  `Opis` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `OznakaNaTerenu` varchar(50) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `GodinaObnove` int DEFAULT NULL,
  `Napomena` longtext CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci,
  `TezinaRute` int NOT NULL,
  `GPXPath` varchar(255) CHARACTER SET utf8mb4 COLLATE utf8mb4_0900_ai_ci DEFAULT NULL,
  `DeletedAt` datetime(6) DEFAULT NULL,
  PRIMARY KEY (`IdRuta`),
  KEY `IX_Rute_IdKontrolnaTocka` (`IdKontrolnaTocka`),
  CONSTRAINT `FK_Rute_KontrolneTocke_IdKontrolnaTocka` FOREIGN KEY (`IdKontrolnaTocka`) REFERENCES `kontrolnetocke` (`IdKontrolnaTocka`) ON DELETE CASCADE
) ENGINE=InnoDB AUTO_INCREMENT=37 DEFAULT CHARSET=utf8mb4 COLLATE=utf8mb4_0900_ai_ci;

/*Data for the table `rute` */

insert  into `rute`(`IdRuta`,`IdKontrolnaTocka`,`Naziv`,`Pocetak`,`Kraj`,`VrijemeHodaMin`,`DuljinaKm`,`VisinskaRazlikaM`,`Opis`,`OznakaNaTerenu`,`GodinaObnove`,`Napomena`,`TezinaRute`,`GPXPath`,`DeletedAt`) values 
(1,1,'Kutina – Humka – Vis','Kutina / Humka','Vrh Vis',90,4.50,260,'Primjer kraće rute do najviše točke Moslavačke gore.','MG-01',2023,'Pogodna za početnike.',0,'C:\\GPX\\ruta_vis.gpx',NULL),
(2,2,'Gračani – Puntijarka – Sljeme','Gračani','Sljeme',150,8.20,780,'Popularan uspon preko Puntijarke prema vrhu Medvednice.','M-04',2022,'Jedna od najčešće korištenih ruta na Medvednici.',1,'C:\\GPX\\ruta_sljeme.gpx',NULL),
(3,3,'Klake – pl. dom pod Okićem – Okić-grad','Klake','Okić – vrh',40,1.80,210,'Najkraći klasični prilaz vrhu Okić preko doma pod Okićem.','SG-01',2021,'Strmiji završni dio prema gradini.',1,'C:\\GPX\\ruta_okic.gpx',NULL),
(4,4,'Šoićeva kuća – Japetić','Šoićeva kuća','Japetić – vrh',90,5.40,430,'Klasičan prilaz preko livada i Katina krča prema vrhu Japetić.','SG-04',2020,'Ruta je pregledna i često korištena.',1,'C:\\GPX\\ruta_japetic.gpx',NULL),
(5,5,'Dom Zavižan – Veliki Zavižan – dom Zavižan','Planinarski dom Zavižan','Veliki Zavižan',150,6.70,320,'Kružna tura s polaskom od doma Zavižan preko Balinovca do Velikog Zavižana.','SV-02',2024,'U nepovoljnim uvjetima potreban dodatni oprez.',2,'C:\\GPX\\ruta_zavizan.gpx',NULL),
(6,8,'Jankovac – Ivačka glava','Jankovac','Ivačka glava',120,5.50,530,'Pristup Papuku od planinarskog doma Jankovac kroz bukovu šumu do najvišeg slavonskog vrha.','PP-01',2023,'Dobro markiran put kroz park prirode Papuk.',1,'C:\\GPX\\ruta_ivacka.gpx',NULL),
(7,9,'Brestovac – Brezovo polje','Brestovac','Brezovo polje',150,7.20,620,'Duži pristup Psunju iz sela Brestovac kroz šumu; pogodan za iskusnije planinare.','PS-01',2022,'Slabije markiran u gornjem dijelu.',1,'C:\\GPX\\ruta_psunj.gpx',NULL),
(8,12,'Ivanec – Ivanščica vrh','Ivanec','Ivanščica',180,9.00,780,'Klasičan pristup najvišem vrhu Zagorja iz Ivanca preko planinarske kuće.','IZ-02',2023,'Dug, ali dobro markiran put.',1,'C:\\GPX\\ruta_ivanscica.gpx',NULL),
(9,14,'Radoboj – Sušec','Radoboj','Sušec',90,4.00,450,'Kraći pristup vrhu Strahinjščice iz Radoboja kroz šumu.','SH-01',2021,'Pogodan za poluizlete.',0,'C:\\GPX\\ruta_susec.gpx',NULL),
(10,15,'Šestine – Grohot','Šestine','Grohot',60,3.20,280,'Kratak uspon od Šestina do vrha Grohot na Medvednici.','MED-07',2024,'Idealan za kratke popodnevne ture.',0,'C:\\GPX\\ruta_grohot.gpx',NULL),
(11,17,'Šestinski dol – Medvedgrad','Šestinski dol','Medvedgrad',45,2.50,300,'Kratak ali strm pristup utvrdi Medvedgrad s južne strane.','MED-02',2024,'Popularna obiteljska ruta.',0,'C:\\GPX\\ruta_medvedgrad.gpx',NULL),
(12,18,'Poljanica – Plešivica','Poljanica Samoborska','Plešivica – vrh',75,4.00,420,'Pristup Plešivici s južne strane iz Poljanice kroz vinograde i šumu.','SG-02',2021,'Lijep pogled na vinograde tijekom uspona.',0,'C:\\GPX\\ruta_plesivica.gpx',NULL),
(13,19,'Japetić dom – Oštrc','Planinarski dom Žitnica','Oštrc – vrh',60,3.00,280,'Grebenski prijelaz od doma Žitnica kod Japetića do vrha Oštrc preko kapele Sv. Ane.','SG-03',2022,'Atraktivan grebenski put s pogledima.',1,'C:\\GPX\\ruta_ostrc.gpx',NULL),
(14,21,'Budinjak – Sveta Gera','Budinjak','Sveta Gera',180,8.50,650,'Dugačak pristup najvišem vrhu Žumberačke gore iz Budinjaka.','ZG-01',2021,'Potrebna dobra kondicija za dulji uspon.',2,'C:\\GPX\\ruta_svetagera.gpx',NULL),
(15,25,'Bjelsko – Klek','Bjelsko','Klek – vrh',120,4.50,780,'Klasičan pristup Kleku iz sela Bjelsko; strm završni dio uz pomoć sajli.','GK-01',2023,'Završni dio zahtijeva osnovnu opremu i iskustvo.',2,'C:\\GPX\\ruta_klek.gpx',NULL),
(16,26,'Begovo Razdolje – Bjelolasica','Begovo Razdolje','Bjelolasica – Kula',90,5.00,430,'Pristup najvišem vrhu Gorskog kotara iz Begovog Razdolja.','GK-05',2024,'Relativno lagodan pristup s makadama.',1,'C:\\GPX\\ruta_bjelolasica.gpx',NULL),
(17,28,'Crni Lug – Risnjak','Crni Lug','Risnjak – vrh',150,7.00,680,'Klasičan pristup Risnjaku iz Crnog Luga kroz nacionalni park.','GK-08',2024,'Prolaz kroz NP Risnjak; plaćanje ulaznice.',1,'C:\\GPX\\ruta_risnjak.gpx',NULL),
(18,29,'Platak – Snježnik','Platak','Snježnik – vrh',120,5.50,510,'Pristup Snježniku s Platka preko planinskog doma.','GK-10',2023,'Može imati snijega do kasnog proljeća.',1,'C:\\GPX\\ruta_snjeznik.gpx',NULL),
(19,31,'Poklon – Vojak','Poklon','Učka – Vojak',90,4.20,520,'Najpopularniji pristup Vojaku s prijevoja Poklon; dobro markiran.','IS-01',2024,'Najpopularnija ruta na Učki.',0,'C:\\GPX\\ruta_vojak.gpx',NULL),
(20,33,'Alan – Mali Rajinac','Planinarski dom Alan','Mali Rajinac',180,8.00,650,'Zahtjevan pristup jednom od najviših velebitskih vrhova iz doma Alan.','SV-03',2023,'Ozbiljan krški teren; potrebna dobra oprema.',2,'C:\\GPX\\ruta_mrajinac.gpx',NULL),
(21,36,'Starigrad Paklenica – Vaganski vrh','Starigrad-Paklenica','Vaganski vrh',360,14.00,1550,'Dugi i zahtjevni uspon na najviši vrh Velebita kroz NP Paklenica.','JV-01',2024,'Cijeli dan hoda; potrebna odlična kondicija.',2,'C:\\GPX\\ruta_vaganski.gpx',NULL),
(22,38,'Velika Paklenica – Anića kuk','Velika Paklenica ulaz','Anića kuk – vrh',120,3.50,500,'Pristup Anića kuku iz klanca Velike Paklenice; alpinistički značajan vrh.','JV-04',2022,'Završni dio tehnički zahtjevan.',2,'C:\\GPX\\ruta_anicakuk.gpx',NULL),
(23,39,'Glogovac – Ozeblin','Glogovac','Ozeblin',240,10.00,900,'Dugačak pristup najvišem vrhu Like iz sela Glogovac.','LI-01',2021,'Slabije markiran gornji dio; potrebna navigacija.',2,'C:\\GPX\\ruta_ozeblin.gpx',NULL),
(24,41,'Baška – Obzova','Baška','Obzova – vrh',120,5.50,500,'Pristup najvišem vrhu Krka iz Baške; otočni krški teren.','OT-01',2023,'Ljeti ponijeti dovoljno vode.',1,'C:\\GPX\\ruta_obzova.gpx',NULL),
(25,43,'Nerežišća – Vidova gora','Nerežišća','Vidova gora',90,4.50,480,'Pristup najvišem otočnom vrhu iz mjesta Nerežišća; pogled na Zlatni rat.','OT-05',2024,'Popularna turistička ruta s izvrsnim vidikom.',0,'C:\\GPX\\ruta_vidovagora.gpx',NULL),
(26,46,'Glavaš – Dinara (Sinjal)','Glavaš','Dinara (Sinjal)',240,9.00,950,'Klasičan pristup najvišem vrhu Hrvatske iz zaseoka Glavaš iznad Vrlike.','DZ-01',2024,'Obavezna točka za srebrnu značku HPO-a. Zahtjevan pristup.',2,'C:\\GPX\\ruta_dinara.gpx',NULL),
(27,47,'Muć – Svilaja','Muć','Svilaja – vrh',210,9.50,1050,'Dugi pristup vrhu Svilaje iz Muća kroz dalmatinsko zaleđe.','DZ-03',2022,'Zahtjevan uspon po toplom vremenu.',2,'C:\\GPX\\ruta_svilaja.gpx',NULL),
(28,49,'Dugopolje – Ljubljan','Dugopolje','Mosor – Ljubljan',150,6.50,860,'Pristup Mosoru iz Dugopolja s markiranim putom prema vrhu Ljubljan.','DA-02',2023,'Popularna splitska planinarska ruta.',1,'C:\\GPX\\ruta_mosor.gpx',NULL),
(29,51,'Bast – Sv. Jure Biokovo','Bast','Sv. Jure',300,11.00,1600,'Najzahtjevniji pristup Biokovu iz Basta na obali; ogromna visinska razlika.','BI-01',2024,'Iznimno zahtjevna ruta; cijeli dan hoda.',2,'C:\\GPX\\ruta_svjure_biokovo.gpx',NULL),
(30,52,'Makarska – Vošac','Makarska','Vošac – vrh',180,6.00,1300,'Popularan uspon na Biokovo iz Makarske s pogledom na rivijeru.','BI-03',2023,'Strm, ali dobro markiran pristup.',2,'C:\\GPX\\ruta_vosac.gpx',NULL),
(31,54,'Orebić – Sv. Ilija Pelješac','Orebić','Sv. Ilija Pelješac',180,6.50,900,'Pristup najvišem pelješkom vrhu iz Orebića; pogled na Korčulu.','DU-01',2022,'Zahtjevan uspon, posebno ljeti.',2,'C:\\GPX\\ruta_svilija_peljesac.gpx',NULL),
(32,55,'Pridvorje – Sniježnica','Pridvorje','Sniježnica – Ilijin vrh',150,6.00,750,'Pristup najvišem vrhu dubrovačkog zaleđa iz Pridvorja.','DU-02',2021,'Ljeti ponijeti dovoljno vode; manje markacija.',1,'C:\\GPX\\ruta_snijeznica.gpx',NULL),
(33,7,'Našice – Kapovac','Našice','Kapovac',150,7.00,540,'Pristup vrhu Krndije iz Našica preko šumskih putova.','SL-01',2022,'Dulji pristup kroz slavonsku šumu.',1,'C:\\GPX\\ruta_kapovac.gpx',NULL),
(34,22,'Budinjak – Pliješ','Budinjak','Pliješ – vrh',120,5.50,500,'Pristup Pliješu iz Budinjaka kroz Žumberačku goru.','ZG-02',2023,'Umjeren pristup šumskim putevima.',1,'C:\\GPX\\ruta_plijes.gpx',NULL),
(35,59,'Kalnik selo – Vranilac','Kalnik (selo)','Vranilac – vrh',90,3.50,340,'Pristup Kalniku iz istoimenog sela; strm završni dio uz stijene.','ZA-03',2022,'Završni dio zahtijeva pažnju.',1,'C:\\GPX\\ruta_vranilac.gpx',NULL),
(36,33,'Alan – Mali Rajinac','Planinarski dom Alan','Mali Rajinac',180,8.00,NULL,NULL,NULL,NULL,NULL,0,NULL,'2026-05-31 15:17:29.133551');

/*!40101 SET SQL_MODE=@OLD_SQL_MODE */;
/*!40014 SET FOREIGN_KEY_CHECKS=@OLD_FOREIGN_KEY_CHECKS */;
/*!40014 SET UNIQUE_CHECKS=@OLD_UNIQUE_CHECKS */;
/*!40111 SET SQL_NOTES=@OLD_SQL_NOTES */;
