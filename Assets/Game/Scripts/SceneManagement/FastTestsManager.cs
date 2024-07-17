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
            2 //Correct answer
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
            "Кто стали представителями Земских соборов в эпоху правления первых Романовых?",
            "Крестьяне и рабочие",
            "Посадские люди и дворяне",
            "Купцы и ремесленники",
            "Бояре",
            2 //Correct answer
            );
        AllFastTests.Add(currentTest);

    }

}
