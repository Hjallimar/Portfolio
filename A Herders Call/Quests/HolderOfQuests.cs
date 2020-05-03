using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//Main Author: Hjalmar Andersson
//Secodary Author: Victor Fagerström

    /// <summary>
    /// I do not belive this script is beeing used anymore
    /// </summary>
public class HolderOfQuests : MonoBehaviour
{

    [SerializeField] static private Dictionary<int, Quest> quests = new Dictionary<int, Quest>();

    // Start is called before the first frame update
    void Start()
    {
        //Add( ID, new Quest( ID, Reward number, dialog, dialog, dialog, namn, ne QuestGoal(typ av quest, goal, vad du börjar på för progress)
        //GoalType - Escort(kossor(n,0)), Gather(what ever(n,n)), Sing(kulning(1,0)), Find(hitta något(1, 0))'
        // rad byten  + "\n"
        //Test Quest

        //Tutorial
        //reward 10 är en fackla
        quests.Add(1, new Quest(1, 10
        , "Vilken ära att få hjälpa den nya koherden att förfylla sitt uppdrag. Ser du kon där borta?" + 
        "Den har inte sprungit långt ifrån sin hage, kan du hämta in den igen? Se upp för vättarna däremot. De tycker om att reta korna så att de blir skrämda." +
        "Ge dem en spark vetja!\nAnvänd 1 för att locka på kor och 2 för att lämna in den när den är i hagen.\nAnvänd E för interaktion/spark." 
        , "Fint jobbat, Frigga. En naturbegåvning! Ta den här facklan. På natten blir det mörkt och det kan hända att du stöter på vargar." +
        "Använd facklan för att skrämma bort vargen. Tänk på att en fackla bara kan användas en gång."+
        "\nAnvänd F för att tända din fackla."
        , "Skynda dig så att hon inte springer iväg för långt!"
        , "Ny på jobbet", 
        new QuestGoal(GoalType.Escort,1 ,0)));

        //Runan ges
        //reward 3 är lokaliseraKoRunan
        quests.Add(2, new Quest(2, 3 
        , "Jag antar att Ragnar lämnade dig sin bok? Ragnar var den bästa herden vi haft och hans arv blir inte lätt att ta efter." + 
        "Utöver att vara herde tog han också del av flera individers historier och dokumenterade dem i den boken du håller i nu." +  
        "Jag uppmuntrar dig till att hitta sidorna som rivits ur då du definitivt träffa anmärkningsvärda karaktärer på vägen." + 
        "Du kanske kan börja hos änkan som bor vid hagen öst om byn.\nAnvänd J för att öppna boken." +
        "För att göra din resa lite lättare har jag en gåva till dig, Frigga."
        , "Med den här runan kan din kulning få svar från kor som är i närheten, då blir det lättare för dig att hitta dom." +  
        "Legender talar om att det finns flera runor ute i världen.\nAnvänd Q för att använda en runa.\nAnvänd TAB för att bläddra mellan runor." + 
        "Om du hamnar vilse på ön kan du använda dig av din kompass för att lättare veta hur du tar dig tillbaka." +
        "Du kan även använda din journal för att se över dina aktuella uppdrag.\nAnvänd L för att öppna journalen." +  
        "\nLycka till, Frigga!"
        , ""
        , "", 
        new QuestGoal(GoalType.Find,0,0)));

        //Hjälp Änkan
        //reward 4 är kapitel 1 //Kräver tutorial (1)
        quests.Add(3, new Quest(3, 4 
        , "Du är byns nya herde eller hur? Frigga var det va? Det märks att du har hjälpt byn väl, deras kor är säkra och välmående. " + 
        "Jag undrar om nästa ko du hämtar in kanske kan få komma till vår hage? " + 
        "Jag och min familj bor här utanför byn och försöker livnära oss på odling men skördarna har inte varit snälla mot oss på sistone så vi behöver lite extra hjälp."
        , "Tack, min vän. Gudarna vet att det inte har varit en lätt vinter. Här är ett litet tecken på min uppskattning." +
        "Något nordväst om byn ligger en liten sjö. Näckens sjö. Näcken har inte alltid varit det han är idag, hans historia är mycket tragisk." + 
        "Kan du göra mig en tjänst och hälsa på honom åt mig? Han är mycket ensam nu för tiden. Tack för all din hjälp, Frigga!"
        , "Har du hittat en ko åt oss än?\nIngen stress..."
        , "Hjälp Änkan", 
        new QuestGoal(GoalType.Escort,1 ,0)));

        //Näkken SJUNG
        //Kräver tutorial (1)
        quests.Add(4, new Quest(4, 0  
        ,"Sjung… Sjung… Sjung! SJUNG! SJUUUUUNG! Sjung för hjärta o själ!"
        ,"Håhåhåhå vilken pipa! Vilken… PIPA!!!"
        ,"Nej, nej nej. Fel tonart. Nej, nej! Helt fel!"
        ,"Sjung!",
        new QuestGoal(GoalType.Song, 1, 0)));

        //Näkken HÄMTA STRÅKE
        //Kräver SJUNG (4)
        quests.Add(5, new Quest(5, 0 
        ,"Med en röst som din måste du vara byns nya herde. Säg mig, är du villig att hjälpa ett åldrande väsen?" + 
        "För många år sedan blev jag av med min stråke. Den bör ligga vid den stora sjön som ligger nordöst på ön, nära berget." +  
        "\nKan du hämta den åt mig?"
        ,"Perfekt! Tack så mycket. Nu slipper jag spela pizzicato som en barbar resten av mitt liv."
        ,"Stråken, har du hittat den? Kolla vid stora sjön, nordöst på ön"
        ,"Näkkens Stråke",
        new QuestGoal(GoalType.Find, 1, 0)));

        //Näkken HÄMTA KO
        //Kräver HÄMTA STRÅKE (5) 
        //Reward 5 är kapitel 2
        quests.Add(6, new Quest(6, 5 
        , "Kan du göra mig en tjänst till när du ändå är farten? Det har samlats lite kor här vid dammen." +  
        "Troligen så dras de sig till musiken, ungefär så som du gör när du vallar dem. De stör min musik!." + 
        "\n Skulle inte du kunna ta dessa kor och lämna dem i byn eller liknande, bara de kommer bort härifrån. Ifall du utgärdar detta så ska du få något mycket intressant." 
        , "Ahhh, det är så lungt och tyst nu när de är borta. Jag kan äntligen spela utan att bli störd. \nTack, Frigga." + 
        "Som utlovat, mitt kapitel. De där sidorna innehåller min historia. Den skrevs ner för många år sedan av en mycket speciell person. Värna om dem, min vän."
        ,"Har du låst in dem någonstans än? Jag vill inte att de ska springa tillbaka hit senare bara för att de hör mig spela."
        ,"De som stör musiken",
        new QuestGoal(GoalType.Escort, 2, 0)));

        //Skuggan JARLENS RING
        // Kräver TUTORIAL 
        //Reward 1 är Blixtrunan
        quests.Add(7, new Quest(7, 1  
        , "Stjäl jarlens ring."
        , "..."
        , "Stjäl jarlens ring."
        , "Jarlens Ring",
        new QuestGoal(GoalType.Find, 1, 0))); 

        //Skuggan SNO JÄTTENS KO
        //Kräver JARLENS RING(7) 
        //Reward 6 är kapitel 3
        quests.Add(8, new Quest(8, 6 
        , "Fria jättens ko"
        , "Allt har mening. Sök träsket i öst"
        , "Fria jättens ko"
        , "Jättens ko",
        new QuestGoal(GoalType.Escort, 1, 0)));

        FillQuestListWithQuests();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void FillQuestListWithQuests()
    {
        QuestComponent.AddRealQuests(quests);
    }
}
