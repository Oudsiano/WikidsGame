using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections;
using UnityEngine;

public class OneFastTest
{
    public int Id { get; set; }
    public string QuestionText { get; set; }
    public string Answer1 { get; set; }
    public string Answer2 { get; set; }
    public string Answer3 { get; set; }
    public string Answer4 { get; set; }
    public int CorrectAnswerIndex { get; set; }

    // Конструктор
    public OneFastTest(int id, string questionText, string answer1, string answer2, string answer3, string answer4, int correctAnswerIndex)
    {
        Id = id;
        QuestionText = questionText;
        Answer1 = answer1;
        Answer2 = answer2;
        Answer3 = answer3;
        Answer4 = answer4;
        CorrectAnswerIndex = correctAnswerIndex;
    }
}

public class FastTestsManager
{

    public List<OneFastTest> AllFastTests;

    public void init()
    {
        AllFastTests = new List<OneFastTest>();
        OneFastTest currentTest;
        //сцена 1 1 и 2 NPC
        currentTest = new OneFastTest(
            1,
            "Какие темы были затронуты в сказках о том времени?",
            "Темы о современных политических событиях",
            "Темы о магии и волшебстве",
            "Темы о патриотизме, крепостничестве, и опричнине",
            "Темы о волшебстве,рабстве и романтике",
            3 //Correct answer
            );
        AllFastTests.Add(currentTest);

        currentTest = new OneFastTest(
            2,
            "Какие символы царской власти упоминались в сказках?",
            "Меч и щит",
            "Корона и трон",
            "Шапка Мономаха и скипетр",
            "Корона и мантия",
            3 //Correct answer
            );
        AllFastTests.Add(currentTest);

        currentTest = new OneFastTest(
            3,
            "В чем выражена мудрость народа в пословицах, поговорках, песнях и загадках?",
            "В живой, меткой и острой речи",
            "В предсказаниях будущего",
            "В рассказах о героических подвигах",
            "В хитрости и лени",
            1 //Correct answer
            );
        AllFastTests.Add(currentTest);

        currentTest = new OneFastTest(
            4,
            "Кто был известным мастером грамоты, обучавшим детей и взрослых?",
            "Александр Свирский",
            "Царь Иван Грозный",
            "Зосима Соловецкий",
            "Соловей Зосимский",
            3 //Correct answer
            );
        AllFastTests.Add(currentTest);

        currentTest = new OneFastTest(
            5,
            "Какая часть культуры была важной для всего народа?",
            "Искусство ваяния",
            "Музыкальные традиции",
            "Бросание топора",
            "Обучение грамоте",
            4 //Correct answer
            );
        AllFastTests.Add(currentTest);

        currentTest = new OneFastTest(
            6,
            "Какую букву царь Иван Грозный ставил на первое место, когда говорил о себе?",
            "А",
            "Б",
            "Я",
            "Э",
            1 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            7,
            "Какие книги читали после изучения азбуки?",
            "Часослов, письмо, Псалтирь и другие церковные книги",
            "Романы и рассказы",
            "Энциклопедии",
            "Научные труды",
            1 //Correct answer
            );
        AllFastTests.Add(currentTest);
        //сцена 1 3 и 4 NPC
        currentTest = new OneFastTest(
            8,
            "Какое важное изобретение помогло земельным учетам, налогам?",
            "Паровой двигатель",
            "Пасхалии – таблицы с указанием дат Пасхи и других праздников",
            "Телеграф",
            "Телефон",
            2 //Correct answer
            );
        AllFastTests.Add(currentTest);

        currentTest = new OneFastTest(
            9,
            "Какие знания были необходимы литейщикам пушек и пищалей?",
            "Знание искусства",
            "Физика для расчета температуры плавления и состава металлов",
            "География",
            "Медицинские знания",
            2 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            10,
            "Для чего была нужна химия в то время?",
            "Для кулинарии",
            "Для выращивания растений",
            "Для солеваров, аптекарей и иконописцев",
            "Для зельеварения",
            3 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            11,
            "Какое знание требовалось путешественникам?",
            "Знание античной литературы",
            "Знание истории",
            "Знание географии",
            "Знание искусства",
            3 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            12,
            "Для чего было необходимо знание механики и математики при строительстве храма Василия Блаженного?",
            "Для расчетов и предотвращения ошибок в строительстве",
            "Для ораторских выступлений",
            "Для создания законов",
            "Для составления поэтических описаний",
            1 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            13,
            "Какой путеводительный материал по солеварению упоминается?",
            "«Солеварские рецепты»",
            "«Роспись, как зачать делать новая труба в новом месте»",
            "«Искусство кулинарии»",
            "«Искусство зельеварения»",
            2 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            14,
            "Кто из персонажей писал о Константинополе, горе Афон, Иерусалиме, Египте?",
            "Иван Грозный",
            "Ф.И. Карпов",
            "Василий Поздняков",
            "Геннадий Аристов",
            3 //Correct answer
            );
        AllFastTests.Add(currentTest);
        //сцена 1 5 и 6 NPC
        currentTest = new OneFastTest(
            15,
            "Какой летописный свод был украшен 16 тысячами миниатюр – лиц?",
            "Воскресенский",
            "Никоновский",
            "Лицевой",
            "Николаевский",
            3 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            16,
            "Какое здание было построено в 1532 году в честь рождения Ивана Грозного?",
            "Стена Китай-города",
            "Церковь Вознесения в Коломенском",
            "Собор Василия Блаженного",
            "Собор Николаевский",
            2 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            17,
            "Каким образом назывались большие и просторные сооружения, строившиеся из дерева?",
            "Храмы",
            "Хоромы",
            "Купола",
            "Дворцы",
            2 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            18,
            "Какое строение было построено после взятия Казани и имело девять шатровых построек?",
            "Стена Белого города",
            "Московский Кремль",
            "Собор Василия Блаженного",
            "Исакиевский собор",
            3 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            19,
            "Какой иконописец прославился в начале XVI века, работая в России?",
            "Андрей Рублёв",
            "Марок",
            "Феофан Грек",
            "Дионисий",
            4 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            20,
            "Какие страны представляли иностранные архитекторы, приглашенные для строительства в конце XV века?",
            "Германия и Франция",
            "Италия",
            "Англия и Испания",
            "Греция и Турция",
            2 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            21,
            "Какие страны представляли иностранные архитекторы, приглашенные для строительства в конце XV века?",
            "Германия и Франция",
            "Италия",
            "Англия и Испания",
            "Греция и Турция",
            2 //Correct answer
            );
        AllFastTests.Add(currentTest);
        //сцена 1 NPC 7 и 8
        currentTest = new OneFastTest(
            22,
            "Какое событие послужило поводом для создания иконы «Церковь воинствующая»?",
            "Возведение Кремля",
            "Создание двигателя",
            "Стоглавый собор",
            "Взятие Казани",
            4 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            23,
            "Какие художественные произведения были названы в честь купцов Строгановых?",
            "Фрески и иконы",
            "Книжные миниатюры",
            "Строения Кремля",
            "Взятие Казани",
            1 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            24,
            "Какие техники использовались для изготовления окладов для книг и икон?",
            "Гравюра и акварель",
            "Штриховка и литография",
            "Филигрань и эмаль",
            "Заготовка и крашение",
            3 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            25,
            "Кто был представителем строгановской школы?",
            "Иван Фёдоров",
            "Прокопий Чирин",
            "Рублев",
            "Дионисий",
            2 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            26,
            "Какие изделия мастерская князей Старицких производила?",
            "Жемчужные украшения",
            "Шитые изделия из шёлка",
            "Металлические ограды",
            "Золотые украшения",
            2 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            27,
            "Какие предметы часто хранились в подпольях домов богатых людей?",
            "Жемчужные украшения",
            "Шитые изделия из шёлка",
            "Металлические ограды",
            "Золотые украшения",
            2 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            28,
            "Что использовали богатые люди для приправления своей пищи?",
            "Соль и перец",
            "Перец и чеснок",
            "Чеснок и лук",
            "Заморские пряности, такие как шафран и корица",
            4 //Correct answer
            );
        AllFastTests.Add(currentTest);
        //сцена 1 NPC 9 и 10

        currentTest = new OneFastTest(
            29,
            "Какие материалы использовались для покрытия крыш изб крестьян?",
            "Слюда и хрупкий камень",
            "Черепица",
            "Солома",
            "Сукно и бараньи меха",
            3 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            30,
            "Что входило в обстановку избы простых крестьян?",
            "Бани и окна из слюды",
            "Горница и клети",
            "Телевизор",
            "Иконы, украшения, одежда",
            4 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            31,
            "Какие инструменты использовались на народных гуляниях?",
            "Пианино и скрипка",
            "Балалайка, гусли, гудок",
            "Труба и виолончель",
            "Губная гармошка",
            2 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            32,
            "Какие события обычно проводились на свадьбах?",
            "Маскарады и представления",
            "Проводы зимы и лета",
            "Праздничный базар",
            "Девишник и сватовство",
            4 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            33,
            "Кто был излюбленным героем народных сборищ и шутов?",
            "Петрушка",
            "Свирельщик",
            "Дудочник",
            "Гармонист",
            1 //Correct answer
            );
        AllFastTests.Add(currentTest);
        //сцена 2 NPC 1 и 2

        currentTest = new OneFastTest(
            34,
            "Кто был первым царём из династии Романовых?",
            "Михаил Фёдорович Романов",
            "Пётр I",
            "Иван Грозный",
            "Николай 2",
            1 //Correct answer
            );
        AllFastTests.Add(currentTest);

        currentTest = new OneFastTest(
            35,
            "Сколько детей родилось у царя Михаила Фёдоровича Романова?",
            "3",
            "2",
            "1",
            "4",
            4 //Correct answer
            );
        AllFastTests.Add(currentTest);

        currentTest = new OneFastTest(
            36,
            "Как звали регента при малолетних братьях царя Алексея?",
            "Екатерина",
            "Софья",
            "Анна",
            "Диана",
            2 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            37,
            "Какое звание имел Алексей Михайлович?",
            "Царь и Великий Князь всея Руси",
            "Король",
            "Император",
            "Герцог",
            1 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            38,
            "Как называлось собрание знатных людей, которое помогало царю править страной?",
            "Кабинет министров",
            "Боярская дума",
            "Сенат",
            "Госдума",
            2 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            39,
            "Сколько человек входило в состав Боярской думы во второй половине XVII века?",
            "50",
            "200",
            "161",
            "97",
            4 //Correct answer
            );
        AllFastTests.Add(currentTest);
        //сцена 2 NPC 3 и 4
        currentTest = new OneFastTest(
            40,
            "Кто стали представителями Земских соборов в эпоху правления первых Романовых?",
            "Крестьяне и рабочие",
            "Посадские люди и дворяне",
            "Купцы и ремесленники",
            "Бояре",
            2 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            41,
            "Когда начала прекращаться деятельность Земских соборов?",
            "В 1653 году",
            "В 1649 году",
            "В 1648 году",
            "В 1652 году",
            1 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            42,
            "Какой приказ обеспечивал почтовые связи для нужд государства?",
            "Поместный приказ",
            "Ямской приказ",
            "Разрядный приказ",
            "Земский приказ",
            2 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            43,
            "Как назывался приказ, который наряжал и разряжал служилых людей?",
            "Поместный приказ",
            "Ямской приказ",
            "Разрядный приказ",
            "Земский приказ",
            3 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            44,
            "Когда было утверждено новое уложение, состоявшее из 25 глав и 967 статей?",
            "В 1648 году",
            "В 1649 году",
            "В 1647 году",
            "В 1646 году",
            2 //Correct answer
            );
        AllFastTests.Add(currentTest);
        //сцена 2 NPC 5 и 6
        currentTest = new OneFastTest(
            45,
            "Что означает поговорка 'Суд да дело – собака съела'?",
            "До решения суда пройдет много времени",
            "Судебный процесс завершился неудачей",
            "Судебный процесс прошел быстро",
            "Судебный процесс не приведет к решению",
            1 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            46,
            "Сколько различных групп составляли русские войска в XVII веке?",
            "Три",
            "Пять",
            "Четыре",
            "Две",
            1 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            47,
            "Сколько воинов составляло русскую армию к концу XVI века?",
            "100 тыс.",
            "37 тыс.",
            "27 тыс.",
            "15 тыс.",
            2 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            48,
            "Каково было влияние церкви на государственные дела при правлении Михаила Фёдоровича Романова?",
            "Она не имела влияния",
            "Она поддерживала власть царя",
            "Она противостояла власти царя",
            "Она имела нейтралитет",
            2 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            49,
            "Какой была основная инициатива патриарха Филарета?",
            "Начало Смоленской войны с Польшей",
            "Учёт земель России",
            "Улучшение судопроизводства",
            "Война с турцией",
            1 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            50,
            "Что стало больше в церкви при правлении патриарха Филарета?",
            "Спокойствия и порядка",
            "Богатства и власти",
            "Скандалов и взяточничества",
            "Отсутсвия порядка",
            1 //Correct answer
            );
        AllFastTests.Add(currentTest);
        //сцена 2 NPC 7 и 8

        currentTest = new OneFastTest(
            51,
            "Кто возглавил движение ревнителей древностей?",
            "Духовный наставник царя Алексея – Стефан Вонифатьев",
            "Никон",
            "Протопоп Аввакум",
            "Царь Алексей Михайлович",
            1 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            52,
            "Какое движение возглавил протопоп Аввакум?",
            "Ревнители древностей",
            "Старая церковь",
            "Новая церковь",
            "Царское движение",
            2 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            53,
            "Какие изменения были внесены Никоном в церковные обряды?",
            "Земные поклоны заменены на поясные, двоеперстие на троеперстие",
            "Последовательность молитв изменена",
            "Удалены крестные ходы",
            "Изменен порядок пения молитв",
            1 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            54,
            "Куда были отправлены противники Никона?",
            "На службу к царю",
            "В изгнание",
            "В монастыри",
            "В ссылку",
            4 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
            55,
            "Каким образом протопоп Аввакум выступил против Никона?",
            "Организовал революцию",
            "Покинул Россию",
            "Заявил, что царь Алексей Михайлович не благочестивейший и не православнейший",
            "Принял сторону Никона",
            3 //Correct answer
            );
        AllFastTests.Add(currentTest);
        //сцена 2 NPC 9 и 10
        currentTest = new OneFastTest(
            56,
            "Кто был патриархом до отставки Никона?",
            "Никон",
            "Алексей Михайлович",
            "Протопоп",
            "Нет правильного ответа",
            4 //Correct answer
            );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
           57,
           "Какое событие стало причиной отставки Никона?",
           "Смерть патриарха",
           "Изменение политики патриарха",
           "Открытие нового монастыря",
           "Отказ царя ходить на службы, которые вёл Никон",
           4 //Correct answer
           );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
           58,
           "В каком году Никона вызвали на суд?",
           "1666",
           "1681",
           "1675",
           "1663",
           1 //Correct answer
           );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
           59,
           "Какое движение возникло после отставки Никона?",
           "Реформация",
           "Протестантизм",
           "Католицизм",
           "Раскольничество",
           4 //Correct answer
           );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
           60,
           "Когда были признаны ошибочными преследования старообрядцев?",
           "1971",
           "1900",
           "1800",
           "1671",
           1 //Correct answer
           );
        AllFastTests.Add(currentTest);
        //сцена 2 NPC 11 и 12
        currentTest = new OneFastTest(
           61,
           "Какие предметы производились на домашних мануфактурах?",
           "сукно, канаты и верёвки, валяные изделия, разная одежда и обувь",
           "посуда, игрушки, книги",
           "стекло, металл, дерево",
           "пластмасса и пенопласт",
           1 //Correct answer
           );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
           62,
           "Какие районы были наиболее важными для развития мануфактур?",
           "Санкт-Петербург, Москва, Нижний Новгород",
           "Сибирь, Кавказ, Украина",
           "Урал, Олонецкий и Тульско-Каширский край",
           "Поволжье и Алтай",
           3 //Correct answer
           );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
           63,
           "Какие города были особо важными центрами торговли?",
           "Санкт-Петербург, Москва",
           "Вологда, Вятка, Великий Устюг, Орёл, Воронеж, Елец, Нижний Новгород",
           "Казань, Самара, Ростов-на-Дону",
           "Новосибирск и Астрахань",
           2 //Correct answer
           );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
           64,
           "Какие сословия были наиболее важными?",
           "бояре и дворяне",
           "купцы и ремесленники",
           "крестьяне",
           "крепостные",
           1 //Correct answer
           );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
           65,
           "Что такое тягло?",
           "карета для перевозки товаров",
           "плата повинностей",
           "единица измерения",
           "плата за перевозку груза",
           2 //Correct answer
           );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
           66,
           "Какие реформы укрепляли собственность дворян на землю?",
           "государственные реформы",
           "торговые реформы",
           "финансовые реформы",
           "реформы в торговле землей",
           1 //Correct answer
           );
        AllFastTests.Add(currentTest);
        //сцена 2 NPC 13 и 14
        currentTest = new OneFastTest(
           67,
           "Когда произошло восстание, получившее название «Соляной бунт»?",
           "1 июня 1648 года",
           "25 июля 1662 года",
           "15 мая 1670 года",
           "22 мая 1670 года",
           1 //Correct answer
           );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
           68,
           "Какое событие послужило поводом для начала «Соляного бунта»?",
           "Отмена налога на соль",
           "Притеснение приказных людей",
           "Война с Речью Посполитой",
           "Война с Турцией",
           2 //Correct answer
           );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
           69,
           "Где произошло мимолётное восстание, получившее название Медный бунт?",
           "Москва",
           "Рязань",
           "Коломенское",
           "Царицын",
           3 //Correct answer
           );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
           70,
           "Какое событие послужило поводом для начала Медного бунта?",
           "Отмена медных денег",
           "Обесценивание медных монет",
           "Война с Персией",
           "Отсутвие обмена медных монет",
           2 //Correct answer
           );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
           71,
           "Какой атаман возглавил казачьи ватаги и принялся за походы на Волгу и Каспий?",
           "Николай 2",
           "Иван Грозный",
           "Степан Разин",
           "Петр I",
           3 //Correct answer
           );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
           72,
           "В каком году Разин собрал в кругу казаков, где изложил план нового похода?",
           "1667",
           "1669",
           "1668",
           "1670",
           2 //Correct answer
           );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
           73,
           "Как закончился поход Разина?",
           "Захват Москвы",
           "Победа на Каспийском море",
           "Разгром в Астрахани",
           "Побег царя",
           3 //Correct answer
           );
        AllFastTests.Add(currentTest);
        //сцена 2 NPC 15 и 16

        currentTest = new OneFastTest(
           74,
           "Кто начал восстание в Симбирске в 1670 году?",
           "Петр I",
           "Иван Грозный",
           "Степан Разин",
           "Лже Дмитрий",
           3 //Correct answer
           );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
           75,
           "Когда был заключен Бахчисарайский мир между Россией и Крымом?",
           "1681 год",
           "1654 год",
           "1660 год",
           "1658 год",
           1 //Correct answer
           );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
           76,
           "Кто командовал гарнизоном крепости Белая?",
           "Богдан Хмельницкий",
           "князь Волконский",
           "Шеин",
           "Степан Разин",
           2 //Correct answer
           );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
           77,
           "Как называлась война в 1656-1658 годах?",
           "Русско-турецкая война",
           "Русско-польская война",
           "Русско-шведская война",
           "Русско-персидская война",
           2 //Correct answer
           );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
           78,
           "Какое событие отмечено в 1648 году?",
           "Русско-шведская война",
           "Началось освободительное движение под предводительством Богдана Хмельницкого",
           "Был заключен Бахчисарайский мир",
           "Русско-Турецкая война",
           2 //Correct answer
           );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
          79,
          "Кто вошел в ряды восставших под контроль войск Разина?",
          "только крестьяне",
          "бояре",
          "новые слои населения",
          "купцы",
          3 //Correct answer
          );
        AllFastTests.Add(currentTest);
        //сцена 2 NPC 17 и 18,19 и 20
        currentTest = new OneFastTest(
          80,
          "Какие события привели к заключению мира между Россией и Речью Посполитой?",
          "Давление Австрии и неудачи в борьбе с Турцией",
          "Вмешательство Франции и Германии",
          "Соглашение с Швецией и Турцией",
          "Давление Германии и Швеции",
          1 //Correct answer
          );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
          81,
          "Какое событие привело к разрыву договора России с Крымом и Турцией?",
          "Вторжение Польши в Россию",
          "Восстание на Украине",
          "Соглашение с Швецией и Турцией",
          "Заключение мира с Речью Посполитой",
          4 //Correct answer
          );
        AllFastTests.Add(currentTest);

        currentTest = new OneFastTest(
          82,
          "Кто открыл пролив между Азией и Америкой?",
          "Ф. Попов",
          "С. Дежнёв",
          "А. Попов",
          "И. Курганов",
          2 //Correct answer
          );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
          83,
          "Когда были разграничены владения Китая и России в Приамурье?",
          "1675 год",
          "1689 год",
          "1685 год",
          "1673 год",
          2 //Correct answer
          );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
         84,
         "Чем занимались жители Кольского полуострова?",
         "Охота и рыбалка",
         "Земледелие и скотоводство",
         "Горное дело",
         "Ремесло",
         1 //Correct answer
         );
        AllFastTests.Add(currentTest);
        //сцена 3 NPC 1 и 2

        currentTest = new OneFastTest(
         85,
         "Кто взошел на престол после смерти царя Алексея?",
         "Фёдор",
         "Иван",
         "Пётр",
         "Николай",
         1 //Correct answer
         );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
         86,
         "Какие языки знал царь Фёдор?",
         "Немецкий и французский",
         "Латинский и польский",
         "Турецкий и польский",
         "Английский и немецкий",
         2 //Correct answer
         );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
         87,
         "В каком году была проведена перепись населения для упрощения сбора налогов?",
         "1692",
         "1682",
         "1676",
         "1654",
         3 //Correct answer
         );
        AllFastTests.Add(currentTest);

        currentTest = new OneFastTest(
         88,
         "Кто был во главе комиссии по военным реформам?",
         "Симеон Полоцкий",
         "Олег Хмельницкий",
         "Николай 2",
         "Боярин князь Голицын",
         4 //Correct answer
         );
        AllFastTests.Add(currentTest);

        currentTest = new OneFastTest(
         89,
         "Как звали первую жену царя Фёдора?",
         "Агафья Грушницкая",
         "Марфа Апраксина",
         "Софья Ивановна",
         "Ольга Голицина",
         1 //Correct answer
         );
        AllFastTests.Add(currentTest);

        currentTest = new OneFastTest(
         90,
         "Какое событие произошло 30 апреля 1682 года?",
         "Была проведена перепись населения",
         "Стрельцы потребовали 16 военных командиров",
         "Медный Бунт",
         "Упрощение сбора налогов",
         2 //Correct answer
         );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
         91,
         "Какое событие произошло 30 апреля 1682 года?",
         "Была проведена перепись населения",
         "Стрельцы потребовали 16 военных командиров",
         "Медный Бунт",
         "Упрощение сбора налогов",
         2 //Correct answer
         );
        AllFastTests.Add(currentTest);
        //сцена 3 NPC 3 и 4
        currentTest = new OneFastTest(
         92,
         "Кто стал регентом после того, как Ивана и Петра сделали царями?",
         "Василий Голицыня",
         "Патреон Беновский",
         "Софья Алексеевна",
         "Ф.Л. Шакловитый",
         3 //Correct answer
         );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
         93,
         "Какое восстание иногда называют словом «Хованщина»?",
         "Стрелецкое восстание",
         "Восстание Пугачева",
         "Бунт Стеньки Разина",
         "Медный бунт",
         1 //Correct answer
         );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
         94,
         "Какое учебное заведение было основано при Софье Алексеевне?",
         "Московский государственный университет",
         "Славяно-греко-латинская академия",
         "Петербургская академия наук",
         "Суворовское училище",
         2 //Correct answer
         );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
         95,
         "Кто был назначен главой правительства во время правления Софьи?",
         "князь Ф.Л. Шакловитый",
         "князь Дмитрий Пожарский",
         "князь Долгорукий",
         "князь Василий Голицын",
         4 //Correct answer
         );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
         96,
         "Что хотел ввести Василий Голицын в Русское государство?",
         "Европейские законы",
         "Систему самоуправления",
         "Монархическую власть",
         "Коммунизм",
         1 //Correct answer
         );
        AllFastTests.Add(currentTest);
        //сцена 3 NPC 5 и 6
        currentTest = new OneFastTest(
         97,
         "В каком году Василий Голицын участвовал в подавлении стрелецкого восстания?",
         "1687",
         "1690",
         "1682",
         "1697",
         3 //Correct answer
         );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
         98,
         "Какое событие стало значимым в деятельности Василия Голицына?",
         "Победа в Крымских походах",
         "Основание Санкт-Петербурга",
         "Заключение «Вечного мира» с Польшей",
         "Война с Персией",
         3 //Correct answer
         );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
         99,
         "Что Россия получила в результате «Вечного мира»?",
         "Левобережную Украину и Киев",
         "Доступ к Черному морю",
         "Контроль над Балтийским морем",
         "Контроль над Каспийским морем",
         1 //Correct answer
         );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
         100,
         "Какие увлечения были у молодого Петра I?",
         "Садоводство и ландшафтный дизайн",
         "Судоводство",
         "Военные игры и «потехи»",
         "Изучение иностранных языков",
         3 //Correct answer
         );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
         101,
         "С кем Пётр I сближался, несмотря на осуждение окружающих?",
         "С восточными странами",
         "С купечеством",
         "С московской знатью",
         "С иностранцами-неправославными",
         4//Correct answer
         );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
         102,
         "Каков был исход Крымских походов, возглавляемых Голицыным?",
         "Неудачный",
         "Успешный",
         "Привел к значительному расширению территории",
         "Опустошение казны",
         1//Correct answer
         );
        AllFastTests.Add(currentTest);
        //сцена 3 NPC 7 и 8

        currentTest = new OneFastTest(
         103,
         "В каком году Пётр I решил, что может самостоятельно править страной?",
         "1689 год",
         "1703 год",
         "1678 год",
         "1670 год",
         1//Correct answer
         );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
         104,
         "Кто был главным врагом Софьи, претендующей на власть?",
         "Николай II",
         "Иван Грозный",
         "Алексей Михайлович",
         "Пётр I",
         4//Correct answer
         );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
         105,
         "Какое событие стало поворотным моментом в борьбе за власть между Софьей и Петром?",
         "Победа в Крымских походах",
         "Ложная тревога в Кремле",
         "Строительство Санкт-Петербурга",
         "Победа над Речью Посполитой",
         2//Correct answer
         );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
         106,
         "Какое решение приняли стрельцы, когда Софья пыталась использовать их для государственного переворота?",
         "Поддержали Софью",
         "Отказались поддержать заговор",
         "Ушли в Преображенское",
         "Удерживали нейтралитет",
         2//Correct answer
         );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
         107,
         "Куда направился Пётр I после ложной тревоги, чтобы искать совета и защиты?    ",
         "В Исакиевский Собор",
         "В Новодевичий монастырь",
         "В Троице-Сергиев монастырь",
         "В Кремль",
         3//Correct answer
         );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
         108,
         "Каков был исход попытки Софьи удержать власть?",
         "Софья была отправлена в монастырь",
         "Софья стала регентом",
         "Софья победила в борьбе за власть",
         "Софья погибла",
         1//Correct answer
         );
        AllFastTests.Add(currentTest);
        currentTest = new OneFastTest(
         109,
         "Какое событие знаменует начало самостоятельного правления Петра I?",
         "Победа в Русско-Турецкой войне",
         "Победа в Северной войне",
         "Основание Санкт-Петербурга",
         "Казнь Шакловитого и его сообщников",
         4//Correct answer
         );
        AllFastTests.Add(currentTest);
    }


}
