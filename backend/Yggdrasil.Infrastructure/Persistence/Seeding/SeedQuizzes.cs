using Yggdrasil.Domain.Entities;
using Yggdrasil.Domain.Enums;

using static Yggdrasil.Infrastructure.Persistence.Seeding.SeedData;

namespace Yggdrasil.Infrastructure.Persistence.Seeding;

/// <summary>
/// Demo quizzes, each with its questions and comment thread. They are listed in the order they
/// were "written": a quiz's position decides its ids, and questions are stamped a minute apart
/// so they come back in the order they appear here. Mark the correct answer with <see cref="C"/>.
/// </summary>
internal static class SeedQuizzes
{
    public static IReadOnlyList<Quiz> All(IReadOnlyList<Category> categories)
    {
        List<Category> In(params Guid[] ids) => [.. ids.Select(id => categories.Single(c => c.Id == id))];

        Quiz[] quizzes =
        [
            new()
            {
                Title = "Friends trivia (the easy kind)",
                Description =
                    "Rewatched the whole thing over christmas so now you all have to suffer with me. "
                    + "Should be pretty easy if you've seen even one season!",
                Difficulty = Difficulty.Easy,
                OwnerId = AlvaId,
                CreatedAt = On("2026-01-09 20:14"),
                Categories = In(TvShowsId, PopCultureId),
                Questions =
                [
                    Q("What's the name of the coffee shop they always hang out in?",
                        "Central Park", C("Central Perk"), "Java Joe's", "The Daily Grind"),
                    Q("What does Ross do for a living?",
                        "Architect", "History teacher", C("Paleontologist"), "Doctor"),
                    Q("Phoebe's most famous song is about a...",
                        C("Smelly cat"), "Lonely dog", "Grumpy pigeon", "Sad goldfish"),
                    Q("Which soap does Joey get a part in?",
                        "General Hospital", "The Young and the Restless", C("Days of Our Lives"), "All My Children"),
                    Q("Monica works as a...",
                        "Nurse", C("Chef"), "Lawyer", "Interior designer"),
                    Q("What's the name of Ross's monkey?",
                        "George", C("Marcel"), "Bubbles", "Coco"),
                    Q("Chandler's middle name is (this one's mean, sorry)",
                        "Michael", "Matthew", C("Muriel"), "Martin"),
                ],
                Comments =
                [
                    By(AdminId, "2026-01-09 21:03", "First quiz on the site! Welcome everyone, have fun with it."),
                    By(NoraId, "2026-01-10 08:02", "Muriel. I had genuinely forgotten about Muriel."),
                    By(TobbenId, "2026-01-10 17:40", "7/7 and ive never watched a full episode. what does that say about me"),
                    By(AlvaId, "2026-01-10 18:05", "that you absorbed it through the walls like the rest of us haha"),
                    By(EirikId, "2026-09-21 20:05", "Started watching Friends because of this quiz. I am on season 3 now"),
                ],
            },
            new()
            {
                Title = "Capitals people get wrong",
                Description = "Not the biggest city. The capital.",
                Difficulty = Difficulty.Normal,
                OwnerId = JonasId,
                CreatedAt = On("2026-01-12 07:30"),
                Categories = In(GeographyId),
                Questions =
                [
                    Q("Capital of Australia?", "Sydney", "Melbourne", C("Canberra"), "Perth"),
                    Q("Capital of Canada?", "Toronto", C("Ottawa"), "Montreal", "Vancouver"),
                    Q("Capital of Turkey?", C("Ankara"), "Istanbul", "Izmir", "Bursa"),
                    Q("Capital of Brazil?", "Rio de Janeiro", "São Paulo", "Salvador", C("Brasília")),
                    Q("Capital of Morocco?", "Casablanca", "Marrakesh", C("Rabat"), "Fez"),
                    Q("Capital of New Zealand?", "Auckland", C("Wellington"), "Christchurch", "Queenstown"),
                    Q("Capital of Nigeria?", "Lagos", C("Abuja"), "Kano", "Ibadan"),
                    Q("Capital of Myanmar?", "Yangon", "Mandalay", C("Naypyidaw"), "Bago"),
                    Q("Capital of Kazakhstan?", C("Astana"), "Almaty", "Shymkent", "Karaganda"),
                    Q("Capital of Switzerland?", "Zürich", "Geneva", C("Bern"), "Basel"),
                ],
                Comments =
                [
                    By(EirikId, "2026-01-12 12:48", "The Myanmar one I did not know at all. I guessed Yangon"),
                    By(NoraId, "2026-01-13 21:10",
                        "Pedantic note: Switzerland technically has no official capital. Bern is the 'federal city'. "
                        + "I'm aware this makes me insufferable.",
                        edited: "2026-01-13 21:14"),
                    By(JonasId, "2026-01-13 21:32", "Correct. Bern is still the answer."),
                    By(KasperId, "2026-01-15 19:02", "astana was called nur-sultan for like 3 years right? felt like a trap"),
                    By(SigridId, "2026-09-14 16:20", "Used this with my class this week. They were furious about Australia."),
                ],
            },
            new()
            {
                Title = "Space for beginners",
                Description =
                    "I made this one for my 5th graders, but grown-ups are welcome to have a go too. "
                    + "No calculators needed. Good luck!",
                Difficulty = Difficulty.Easy,
                OwnerId = SigridId,
                CreatedAt = On("2026-01-15 16:05"),
                Categories = In(ScienceId),
                Questions =
                [
                    Q("How many planets are there in our solar system?", "7", C("8"), "9", "10"),
                    Q("Which planet is the biggest?", "Saturn", C("Jupiter"), "Neptune", "Earth"),
                    Q("Why does Mars look red?",
                        C("Its ground is full of rusty iron dust"), "It is very close to the Sun",
                        "Its oceans are red", "Its clouds are made of red gas"),
                    Q("Who was the first person to walk on the Moon?",
                        "Buzz Aldrin", "Yuri Gagarin", C("Neil Armstrong"), "Michael Collins"),
                    Q("Which planet is the hottest?", "Mercury", C("Venus"), "Mars", "Jupiter"),
                    Q("About how long does sunlight take to reach Earth?",
                        "8 seconds", C("8 minutes"), "8 hours", "8 days"),
                    Q("What is the name of Saturn's biggest moon?", C("Titan"), "Europa", "Ganymede", "Phobos"),
                ],
                Comments =
                [
                    By(AlvaId, "2026-01-16 19:30", "the venus one got me!! I was so sure it was mercury"),
                    By(SigridId, "2026-01-16 20:02",
                        "Half my class said Mercury too! Venus has a thick atmosphere that traps the heat, "
                        + "so it ends up even hotter."),
                    By(TobbenId, "2026-01-18 10:15", "got beaten by a quiz for 5th graders. great start to the week"),
                ],
            },
            new()
            {
                Title = "world cup basics",
                Description = "warming up for the summer. easy ones, no excuses",
                Difficulty = Difficulty.Easy,
                OwnerId = TobbenId,
                CreatedAt = On("2026-01-19 21:47"),
                Categories = In(SportsId),
                Questions =
                [
                    Q("which country has won the most world cups", "Germany", "Italy", C("Brazil"), "Argentina"),
                    Q("who won the 2022 world cup in qatar", "France", C("Argentina"), "Croatia", "Morocco"),
                    Q("where was the first ever world cup (1930)", C("Uruguay"), "Italy", "Brazil", "England"),
                    Q("2026 has three hosts. which one is NOT hosting", "USA", "Canada", "Mexico", C("Costa Rica")),
                    Q("how long is a normal match, no extra time",
                        "80 minutes", C("90 minutes"), "100 minutes", "120 minutes"),
                    Q("top scorer of the tournament gets the...",
                        "Golden Ball", C("Golden Boot"), "Golden Glove", "Ballon d'Or"),
                ],
                Comments =
                [
                    By(JonasId, "2026-01-20 07:12", "Fair. The last question is the only one with any bite."),
                    By(KasperId, "2026-01-20 12:30", "i put golden ball. in my defence it sounds right"),
                    By(EirikId, "2026-01-23 18:44", "Norway is in it this time!! First World Cup since 1998. I am not calm"),
                    By(TobbenId, "2026-01-23 19:10", "haaland is going to eat"),
                ],
            },
            new()
            {
                Title = "First lines",
                Description =
                    "Name the novel from its opening line. Most of these are famous enough that you'll know them "
                    + "without having read the book. A couple are there purely to make you feel bad about your "
                    + "reading list (they made me feel bad about mine).",
                Difficulty = Difficulty.Expert,
                OwnerId = NoraId,
                CreatedAt = On("2026-01-22 22:15"),
                Categories = In(BooksId),
                Questions =
                [
                    Q("\"Call me Ishmael.\"",
                        C("Moby-Dick"), "Treasure Island", "The Old Man and the Sea", "Robinson Crusoe"),
                    Q("\"It is a truth universally acknowledged...\"",
                        "Emma", "Sense and Sensibility", C("Pride and Prejudice"), "Jane Eyre"),
                    Q("\"All happy families are alike...\"",
                        "War and Peace", C("Anna Karenina"), "Crime and Punishment", "The Brothers Karamazov"),
                    Q("\"It was a bright cold day in April, and the clocks were striking thirteen.\"",
                        C("Nineteen Eighty-Four"), "Brave New World", "Fahrenheit 451", "We"),
                    Q("\"Many years later, as he faced the firing squad...\"",
                        "Love in the Time of Cholera", C("One Hundred Years of Solitude"),
                        "The House of the Spirits", "Pedro Páramo"),
                    Q("\"Mother died today.\"", C("The Stranger"), "The Plague", "Nausea", "The Trial"),
                    Q("\"124 was spiteful.\"",
                        "Song of Solomon", "The Color Purple", "Their Eyes Were Watching God", C("Beloved")),
                    Q("\"It was the best of times, it was the worst of times...\"",
                        "Great Expectations", "Oliver Twist", C("A Tale of Two Cities"), "Bleak House"),
                    Q("\"Someone must have slandered Josef K....\"",
                        "The Castle", C("The Trial"), "The Metamorphosis", "Amerika"),
                ],
                Comments =
                [
                    By(JonasId, "2026-01-23 09:01", "6/9. The Camus one depends a lot on which translation you read."),
                    By(NoraId, "2026-01-23 12:20",
                        "Fair. I went with the one on my shelf, which is about as scientific as this quiz gets."),
                    By(AlvaId, "2026-01-24 20:47", "I got the Jane Austen one and then nothing else. loved it though"),
                ],
            },
            new()
            {
                Title = "n64 nostalgia",
                Description = "blowing into the cartridge was a placebo and i will not be taking questions",
                Difficulty = Difficulty.Normal,
                OwnerId = KasperId,
                CreatedAt = On("2026-01-26 23:41"),
                Categories = In(GamesId),
                Questions =
                [
                    Q("which game launched alongside the n64 in both japan and north america",
                        C("Super Mario 64"), "Mario Kart 64", "GoldenEye 007", "Banjo-Kazooie"),
                    Q("who made goldeneye 007", "Nintendo EAD", C("Rare"), "Retro Studios", "Factor 5"),
                    Q("majora's mask needed which add-on to run",
                        "Rumble Pak", "Transfer Pak", C("Expansion Pak"), "Controller Pak"),
                    Q("what year did ocarina of time come out", "1996", "1997", C("1998"), "1999"),
                    Q("how many handles does the n64 controller have", "2", C("3"), "4"),
                    Q("the blue shell first shows up in mario kart 64. whats it actually called",
                        C("Spiny Shell"), "Buzzy Shell", "Koopa Shell", "Thunder Shell"),
                    Q("banjo is a bear. what is kazooie", "a mole", C("a bird"), "a frog", "a backpack"),
                ],
                Comments =
                [
                    By(TobbenId, "2026-01-27 18:22", "goldeneye with 4 people and someone always picks oddjob"),
                    By(JonasId, "2026-01-27 19:05", "Oddjob was banned in our house."),
                    By(EirikId, "2026-01-28 08:40", "We had only one controller so I watched my brother play for two years"),
                    By(KasperId, "2026-01-28 09:13", "this is the saddest comment on the whole site"),
                ],
            },
            new()
            {
                Title = "Norway for beginners",
                Description =
                    "Some easy questions about Norway. If you live here you must get all right, sorry this is the rule.",
                Difficulty = Difficulty.Easy,
                OwnerId = EirikId,
                CreatedAt = On("2026-01-29 11:20"),
                Categories = In(GeographyId),
                Questions =
                [
                    Q("What is the capital of Norway?", "Bergen", C("Oslo"), "Trondheim", "Stavanger"),
                    Q("When is the national day in Norway?", "6 June", "1 May", C("17 May"), "24 June"),
                    Q("What is the longest fjord in Norway?",
                        C("Sognefjorden"), "Hardangerfjorden", "Geirangerfjorden", "Oslofjorden"),
                    Q("What is the highest mountain in Norway?",
                        "Glittertind", C("Galdhøpiggen"), "Snøhetta", "Store Skagastølstind"),
                    Q("Which Nobel prize is given in Oslo and not in Stockholm?", "Literature", C("Peace"), "Physics"),
                    Q("Which city has Bryggen, the old wharf with the wooden houses?",
                        "Oslo", "Ålesund", "Tromsø", C("Bergen")),
                    Q("What is the money called in Norway?", "Euro", C("Krone"), "Mark", "Daler"),
                ],
                Comments =
                [
                    By(SigridId, "2026-01-29 17:30",
                        "Lovely! The national day one should be easy after all the ice cream we eat that day."),
                    By(KasperId, "2026-01-30 00:12", "the 'you must get all right' rule is very real"),
                    By(NoraId, "2026-01-31 14:55",
                        "Galdhøpiggen vs Glittertind is an old rivalry. Glittertind used to come out taller "
                        + "if you counted the glacier on top."),
                    By(EirikId, "2026-01-31 15:40", "Yes! And now the glacier is melting so the fight is over. Sad way to win"),
                ],
            },
            new()
            {
                Title = "90s movie quotes",
                Description =
                    "Which movie is it from? I wrote these from memory so if one is slightly off "
                    + "that's on purpose (it isn't).",
                Difficulty = Difficulty.Normal,
                OwnerId = AlvaId,
                CreatedAt = On("2026-02-02 19:55"),
                Categories = In(MoviesId, PopCultureId),
                Questions =
                [
                    Q("\"Show me the money!\"", C("Jerry Maguire"), "Wall Street", "The Wolf of Wall Street", "Rain Man"),
                    Q("\"You can't handle the truth!\"", "The Firm", C("A Few Good Men"), "Philadelphia", "JFK"),
                    Q("\"Houston, we have a problem.\"", "Armageddon", "Contact", C("Apollo 13"), "Deep Impact"),
                    Q("\"To infinity and beyond!\"", C("Toy Story"), "A Bug's Life", "The Iron Giant", "Antz"),
                    Q("\"Hasta la vista, baby.\"",
                        "Total Recall", "True Lies", C("Terminator 2: Judgment Day"), "Predator 2"),
                    Q("\"Life was like a box of chocolates...\"",
                        "Big", C("Forrest Gump"), "Groundhog Day", "Good Will Hunting"),
                    Q("\"I'm the king of the world!\"", C("Titanic"), "The Lion King", "Braveheart", "Gladiator"),
                ],
                Comments =
                [
                    By(KasperId, "2026-02-03 01:20", "the 'from memory' disclaimer is doing a lot of work here"),
                    By(AlvaId, "2026-02-03 07:45", "I checked them all!! mostly"),
                    By(JonasId, "2026-02-03 08:10", "Gladiator is from 2000. Not that anyone picked it."),
                ],
            },
            new()
            {
                Title = "Element symbols",
                Description = null,
                Difficulty = Difficulty.Normal,
                OwnerId = JonasId,
                CreatedAt = On("2026-02-05 06:58"),
                Categories = In(ScienceId),
                Questions =
                [
                    Q("Au", "Silver", C("Gold"), "Aluminium", "Argon"),
                    Q("Fe", C("Iron"), "Fluorine", "Fermium", "Francium"),
                    Q("Na", "Nitrogen", "Neon", C("Sodium"), "Nickel"),
                    Q("K", "Krypton", C("Potassium"), "Calcium", "Phosphorus"),
                    Q("Hg", "Hydrogen", "Helium", "Hafnium", C("Mercury")),
                    Q("W", C("Tungsten"), "Vanadium", "Xenon", "Yttrium"),
                    Q("Sn", "Antimony", C("Tin"), "Sulfur", "Selenium"),
                    Q("Pb", C("Lead"), "Platinum", "Palladium", "Polonium"),
                ],
                Comments =
                [
                    By(KasperId, "2026-02-05 22:41", "W for tungsten is a crime"),
                    By(NoraId, "2026-02-06 09:30",
                        "It comes from 'wolfram', for what it's worth. Doesn't make it any less annoying."),
                    By(SigridId, "2026-02-07 12:02", "Sn and Sb always get me. Got it right this time!"),
                ],
            },
            new()
            {
                Title = "Beatles or not?",
                Description =
                    "Just two buttons this time: was it the Beatles, or someone else? "
                    + "My dad would be ashamed of how many of these I had to double check.",
                Difficulty = Difficulty.Easy,
                OwnerId = SigridId,
                CreatedAt = On("2026-02-08 14:30"),
                Categories = In(MusicId),
                Questions =
                [
                    Q("\"Hey Jude\"", C("The Beatles"), "Someone else"),
                    Q("\"Paint It Black\"", "The Beatles", C("Someone else")),
                    Q("\"Penny Lane\"", C("The Beatles"), "Someone else"),
                    Q("\"Norwegian Wood\"", C("The Beatles"), "Someone else"),
                    Q("\"My Generation\"", "The Beatles", C("Someone else")),
                    Q("\"Wonderwall\"", "The Beatles", C("Someone else")),
                    Q("\"Mrs. Robinson\"", "The Beatles", C("Someone else")),
                    Q("\"Here Comes the Sun\"", C("The Beatles"), "Someone else"),
                    Q("\"California Dreamin'\"", "The Beatles", C("Someone else")),
                    Q("\"Ob-La-Di, Ob-La-Da\"", C("The Beatles"), "Someone else"),
                ],
                Comments =
                [
                    By(EirikId, "2026-02-08 18:22", "Norwegian Wood is a good choice :)"),
                    By(TobbenId, "2026-02-09 21:05", "wonderwall not the beatles?? liam would be devastated"),
                    By(AlvaId, "2026-02-10 19:48", "this format is so good, more of these please"),
                ],
            },
            new()
            {
                Title = "speedrun lore",
                Description =
                    "for people who watch gdq at 4am instead of sleeping. "
                    + "if you get all of these you need to go outside (me too)",
                Difficulty = Difficulty.Expert,
                OwnerId = KasperId,
                CreatedAt = On("2026-02-11 01:12"),
                Categories = In(GamesId),
                Questions =
                [
                    Q("what does 'any%' mean",
                        C("beat the game however you can, completion doesn't matter"), "collect every item",
                        "play on any difficulty", "use any version of the game"),
                    Q("what does TAS stand for",
                        "timed any% speedrun", C("tool-assisted speedrun"), "total average score",
                        "tournament approved setup"),
                    Q("the BLJ (backwards long jump) is from which game",
                        "Super Mario Sunshine", C("Super Mario 64"), "Banjo-Kazooie", "Spyro the Dragon"),
                    Q("agdq (the winter marathon) raises money for",
                        C("Prevent Cancer Foundation"), "Doctors Without Borders", "Red Cross", "Child's Play"),
                    Q("and sgdq in the summer?",
                        "Prevent Cancer Foundation", C("Doctors Without Borders"), "Save the Children", "AbleGamers"),
                    Q("in a run, what's a 'split'",
                        "a run with two players", C("your time at a checkpoint"), "a category with two rule sets",
                        "resetting after a bad start"),
                    Q("what timer do pretty much all runners use", "OBS", C("LiveSplit"), "Discord", "the steam overlay"),
                    Q("where are basically all the leaderboards", C("speedrun.com"), "twitch", "reddit", "splits.io"),
                ],
                Comments =
                [
                    By(TobbenId, "2026-02-11 18:30", "understood maybe 40% of this. respect"),
                    By(NoraId, "2026-02-12 21:14", "I came for the vibes and left knowing what a BLJ is. Thank you, I think."),
                    By(EirikId, "2026-02-13 07:50", "GDQ in january is my tradition now, right after christmas"),
                ],
            },
            new()
            {
                Title = "Studio Ghibli",
                Description =
                    "My Valentine's Day plans were a Ghibli marathon and writing a quiz nobody asked for. "
                    + "No plot spoilers; it's trivia about the films rather than what happens in them.",
                Difficulty = Difficulty.Hard,
                OwnerId = NoraId,
                CreatedAt = On("2026-02-14 20:20"),
                Categories = In(MoviesId),
                Questions =
                [
                    Q("Which Ghibli film was the first to win the Oscar for Best Animated Feature?",
                        "My Neighbor Totoro", "Princess Mononoke", C("Spirited Away"), "Howl's Moving Castle"),
                    Q("What's the name of Kiki's black cat?", C("Jiji"), "Calcifer", "Kodama", "Muta"),
                    Q("Howl's Moving Castle is based on a novel by...",
                        "Ursula K. Le Guin", C("Diana Wynne Jones"), "Roald Dahl", "Philip Pullman"),
                    Q("Who directed Grave of the Fireflies?",
                        "Hayao Miyazaki", C("Isao Takahata"), "Gorō Miyazaki", "Hiromasa Yonebayashi"),
                    Q("Which composer has scored nearly all of Miyazaki's films?",
                        C("Joe Hisaishi"), "Ryuichi Sakamoto", "Yoko Kanno", "Hans Zimmer"),
                    Q("At the bus stop in My Neighbor Totoro, what do the sisters lend Totoro?",
                        "A raincoat", C("An umbrella"), "A lantern", "A hat"),
                    Q("Which producer co-founded the studio with Miyazaki and Takahata?",
                        C("Toshio Suzuki"), "Hideaki Anno", "Mamoru Oshii", "Satoshi Kon"),
                    Q("In Porco Rosso, the pilot has been turned into a...", "Fox", C("Pig"), "Crow", "Cat"),
                ],
                Comments =
                [
                    By(AlvaId, "2026-02-14 22:01", "this is literally my valentine's day too. we should start a club"),
                    By(KasperId, "2026-02-15 13:37", "the boy and the heron won too btw"),
                    By(NoraId, "2026-02-15 14:02",
                        "It did! That's why the question says 'first'. I only noticed while writing it, "
                        + "which is how I learn most things."),
                    By(SigridId, "2026-02-16 16:20", "Lovely quiz. My class watched Totoro before Christmas and loved it."),
                ],
            },
            new()
            {
                Title = "premier league nerd round",
                Description = "ok this one is actually hard. no googling, i will know (i won't)",
                Difficulty = Difficulty.Expert,
                OwnerId = TobbenId,
                CreatedAt = On("2026-02-17 22:05"),
                Categories = In(SportsId),
                Questions =
                [
                    Q("all-time top scorer in the premier league",
                        "Wayne Rooney", C("Alan Shearer"), "Harry Kane", "Thierry Henry"),
                    Q("who went the whole 03/04 season unbeaten", C("Arsenal"), "Chelsea", "Man United", "Liverpool"),
                    Q("leicester won it in 15/16. what odds were they at the start of the season",
                        "100-1", "500-1", "1000-1", C("5000-1")),
                    Q("fastest hat-trick in pl history",
                        "Robbie Fowler", C("Sadio Mané"), "Mohamed Salah", "Sergio Agüero"),
                    Q("who won the first ever pl season (92/93)", "Blackburn", "Arsenal", C("Man United"), "Aston Villa"),
                    Q("how many points did city get in 17/18", "95", "97", "98", C("100")),
                    Q("most clean sheets in pl history",
                        C("Petr Čech"), "David de Gea", "Edwin van der Sar", "David James"),
                ],
                Comments =
                [
                    By(JonasId, "2026-02-18 06:55", "Seven questions, I got four. Expert sounds right."),
                    By(KasperId, "2026-02-18 12:12", "5000-1. imagine having that bet slip"),
                    By(TobbenId, "2026-02-18 12:40",
                        "guy at my local had it and cashed out in march. we don't bring it up",
                        edited: "2026-02-18 12:43"),
                    By(EirikId, "2026-02-19 20:15", "The Leicester season was so crazy. I still dont understand it"),
                    By(AdminId, "2026-02-20 09:00",
                        "Keep it civil in here please. Removed a couple of comments that were just club insults."),
                ],
            },
            new()
            {
                Title = "ABBA of course",
                Description = "Swedish neighbours, but we can share. Easy if you have parents.",
                Difficulty = Difficulty.Easy,
                OwnerId = EirikId,
                CreatedAt = On("2026-02-20 18:30"),
                Categories = In(MusicId),
                Questions =
                [
                    Q("Which song won Eurovision for ABBA in 1974?", "Mamma Mia", C("Waterloo"), "Dancing Queen", "SOS"),
                    Q("Which country is ABBA from?", C("Sweden"), "Norway", "Denmark", "Finland"),
                    Q("The name ABBA comes from...",
                        C("The first letters of their first names"), "A Swedish word for 'together'",
                        "Their record label", "A street in Stockholm"),
                    Q("What is the name of the musical with ABBA songs?",
                        "Waterloo!", C("Mamma Mia!"), "Dancing Queen", "Thank You for the Music"),
                    Q("In which English city was Eurovision in 1974?", "London", "Manchester", C("Brighton"), "Liverpool"),
                    Q("In the ABBA Voyage show in London, the band performs as...",
                        C("Digital avatars"), "Holograms from old concerts", "Their children", "A tribute band"),
                ],
                Comments =
                [
                    By(NoraId, "2026-02-20 21:11",
                        "Anni-Frid was born in Norway, so you could claim a quarter of the band if you wanted to."),
                    By(EirikId, "2026-02-20 21:30", "I want this very much. 25% ABBA"),
                    By(SigridId, "2026-02-22 10:05", "My mum would get every single one. I got five!"),
                ],
            },
            new()
            {
                Title = "Pokémon gen 1",
                Description =
                    "For everyone who was 8 in 1999 and thought MissingNo was a real Pokémon (I did) (it's not)",
                Difficulty = Difficulty.Easy,
                OwnerId = AlvaId,
                CreatedAt = On("2026-02-24 21:10"),
                Categories = In(GamesId, PopCultureId),
                Questions =
                [
                    Q("Which of these is NOT a starter in Red and Blue?",
                        "Bulbasaur", "Charmander", "Squirtle", C("Pikachu")),
                    Q("What number is Bulbasaur in the Pokédex?", C("#001"), "#025", "#150", "#151"),
                    Q("What type is Pikachu?", "Normal", C("Electric"), "Fire", "Psychic"),
                    Q("Who gives you your first Pokémon?",
                        "Professor Elm", C("Professor Oak"), "Professor Birch", "Professor Rowan"),
                    Q("How many Pokémon were there in gen 1?", "100", "150", C("151"), "251"),
                    Q("Which Pokémon can turn into whatever it's looking at?", "Mew", C("Ditto"), "Eevee", "Porygon"),
                    Q("In Japan the very first games were Red and...", C("Green"), "Blue", "Yellow", "Gold"),
                ],
                Comments =
                [
                    By(KasperId, "2026-02-24 23:02", "mew under the truck was real to me for years"),
                    By(TobbenId, "2026-02-25 07:31", "squirtle gang"),
                    By(EirikId, "2026-02-26 19:44", "I got Blue for christmas in 1999 I think. Best christmas"),
                ],
            },
            new()
            {
                Title = "The Cold War",
                Description = "Dates and names. Nothing obscure.",
                Difficulty = Difficulty.Normal,
                OwnerId = JonasId,
                CreatedAt = On("2026-02-27 07:45"),
                Categories = In(HistoryId),
                Questions =
                [
                    Q("In which year did the Berlin Wall fall?", "1987", C("1989"), "1990", "1991"),
                    Q("Which satellite did the Soviet Union launch in 1957?",
                        C("Sputnik 1"), "Vostok 1", "Explorer 1", "Luna 2"),
                    Q("Who was the first human in space?",
                        "Alexei Leonov", "Valentina Tereshkova", C("Yuri Gagarin"), "Alan Shepard"),
                    Q("The Cuban Missile Crisis took place in...", "1959", "1961", C("1962"), "1965"),
                    Q("NATO was founded in...", C("1949"), "1945", "1955", "1961"),
                    Q("Who led the Soviet Union when it dissolved?",
                        "Leonid Brezhnev", "Boris Yeltsin", C("Mikhail Gorbachev"), "Yuri Andropov"),
                    Q("Which Apollo 11 astronaut stayed in lunar orbit while the other two landed?",
                        "Buzz Aldrin", C("Michael Collins"), "Jim Lovell", "John Glenn"),
                    Q("The Warsaw Pact was signed in...", "1949", C("1955"), "1961", "1968"),
                ],
            },
            new()
            {
                Title = "f1 quiz before the season",
                Description = "season starts next weekend so here we go. mix of old and new",
                Difficulty = Difficulty.Hard,
                OwnerId = TobbenId,
                CreatedAt = On("2026-03-01 19:30"),
                Categories = In(SportsId),
                Questions =
                [
                    Q("what does DRS stand for",
                        C("Drag Reduction System"), "Downforce Regulation System", "Dynamic Racing Setup",
                        "Driver Response Signal"),
                    Q("schumacher has 7 titles. who else has 7",
                        "Ayrton Senna", C("Lewis Hamilton"), "Sebastian Vettel", "Alain Prost"),
                    Q("where was the first ever f1 championship race (1950)", "Monza", "Monaco", C("Silverstone"), "Spa"),
                    Q("which team calls monza its home race", C("Ferrari"), "McLaren", "Williams", "Red Bull"),
                    Q("what year did verstappen win his first title", "2019", "2020", C("2021"), "2022"),
                    Q("how many points do you get for a win", "10", "18", "20", C("25")),
                    Q("what country was ayrton senna from", "Argentina", "Portugal", C("Brazil"), "Spain"),
                ],
                Comments =
                [
                    By(JonasId, "2026-03-02 07:20", "DRS is gone from this season, isn't it?"),
                    By(TobbenId, "2026-03-02 08:05", "yeah replaced by the new active aero stuff. still counts as trivia!!"),
                    By(KasperId, "2026-03-03 22:50", "said 10 points for a win because of old games. painful"),
                ],
            },
            new()
            {
                Title = "Animal facts that sound made up",
                Description =
                    "True or false? A few of these surprised me when I looked them up. Good one for the dinner table.",
                Difficulty = Difficulty.Easy,
                OwnerId = SigridId,
                CreatedAt = On("2026-03-03 15:15"),
                Categories = In(NatureId),
                Questions =
                [
                    Q("An octopus has three hearts.", C("True"), "False"),
                    Q("Goldfish only remember things for about three seconds.", "True", C("False")),
                    Q("Koalas have fingerprints that look a lot like ours.", C("True"), "False"),
                    Q("Bats are blind.", "True", C("False")),
                    Q("Flamingos get their pink colour from the food they eat.", C("True"), "False"),
                    Q("A group of crows is called a 'murder'.", C("True"), "False"),
                    Q("Ostriches bury their heads in the sand when they are scared.", "True", C("False")),
                    Q("Wombat poo is shaped like little cubes.", C("True"), "False"),
                ],
                Comments =
                [
                    By(AlvaId, "2026-03-03 20:12", "WOMBAT POO?? why did nobody tell me this sooner"),
                    By(KasperId, "2026-03-04 00:40", "bats not being blind ruined my whole childhood"),
                    By(JonasId, "2026-03-04 07:15", "The crow one is true in the dictionary sense. No ornithologist actually says it."),
                ],
            },
            new()
            {
                Title = "internet history",
                Description = "some of this is older than me which is a weird feeling",
                Difficulty = Difficulty.Normal,
                OwnerId = KasperId,
                CreatedAt = On("2026-03-06 00:33"),
                Categories = In(PopCultureId),
                Questions =
                [
                    Q("first video ever uploaded to youtube",
                        C("Me at the zoo"), "Charlie bit my finger", "Evolution of Dance", "Hello World"),
                    Q("rickrolling uses which song",
                        "Take On Me", C("Never Gonna Give You Up"), "Africa", "Together Forever"),
                    Q("the doge dog is what breed", "Akita", C("Shiba Inu"), "Corgi", "Pomeranian"),
                    Q("what year did wikipedia launch", "1998", C("2001"), "2004", "2006"),
                    Q("twitter's original character limit", C("140"), "160", "200", "280"),
                    Q("first youtube video to hit 1 billion views",
                        "Baby", "Despacito", C("Gangnam Style"), "Charlie bit my finger"),
                    Q("what does tl;dr stand for",
                        "total loss; don't retry", C("too long; didn't read"), "talk later; driving right now",
                        "to-do list; done reading"),
                ],
                Comments =
                [
                    By(SigridId, "2026-03-06 16:30", "I had to ask my students about the dog one. They laughed at me."),
                    By(NoraId, "2026-03-07 10:48",
                        "I remember the 140-character era. Everyone was briefer and nobody was any kinder."),
                    By(TobbenId, "2026-03-08 13:22", "gangnam style was 2012?? i feel ancient"),
                ],
            },
            new()
            {
                Title = "Tolkien (books, not films)",
                Description =
                    "Books only. Where the films disagree with the books, the books win; "
                    + "I don't make the rules, Tolkien does.",
                Difficulty = Difficulty.Hard,
                OwnerId = NoraId,
                CreatedAt = On("2026-03-09 21:40"),
                Categories = In(BooksId),
                Questions =
                [
                    Q("Where did Tolkien teach for most of his career?", "Cambridge", C("Oxford"), "Edinburgh", "Leeds"),
                    Q("What is Bilbo's home called?", C("Bag End"), "Bree", "Crickhollow", "Rivendell"),
                    Q("How many dwarves are in Thorin's company, Thorin included?", "7", "12", C("13"), "14"),
                    Q("What is Gandalf's horse called?", "Brego", C("Shadowfax"), "Bill", "Asfaloth"),
                    Q("Which two Elvish languages did Tolkien develop the furthest?",
                        "Khuzdul and Adûnaic", "Westron and Rohirric", C("Quenya and Sindarin"), "Nandorin and Telerin"),
                    Q("What does Bilbo call the book he writes about his adventure?",
                        C("There and Back Again"), "The Road Goes Ever On", "Riddles in the Dark", "Concerning Hobbits"),
                    Q("When was The Hobbit first published?", "1925", C("1937"), "1949", "1954"),
                    Q("What is the inn at Bree called?",
                        "The Green Dragon", C("The Prancing Pony"), "The Golden Perch", "The Ivy Bush"),
                ],
                Comments =
                [
                    By(JonasId, "2026-03-10 06:50", "Leeds as an option on the first question is cruel. Good."),
                    By(AlvaId, "2026-03-10 19:02", "I've only seen the films and got 3. did the films lie to me??"),
                    By(NoraId, "2026-03-10 19:30", "The films are lovely! They just moved a few things around."),
                    By(KasperId, "2026-03-11 23:58", "13 dwarves is so many dwarves"),
                ],
            },
            new()
            {
                Title = "Ancient Egypt",
                Description = "We're doing Egypt this term, so here is a small quiz to go with it.",
                Difficulty = Difficulty.Easy,
                OwnerId = SigridId,
                CreatedAt = On("2026-03-16 14:10"),
                Categories = In(HistoryId),
                Questions =
                [
                    Q("Which river was the most important to ancient Egypt?",
                        C("The Nile"), "The Tigris", "The Euphrates", "The Jordan"),
                    Q("The Great Pyramid of Giza was built for which pharaoh?",
                        "Tutankhamun", C("Khufu"), "Ramesses II", "Cleopatra"),
                    Q("Who found Tutankhamun's tomb in 1922?",
                        "Jean-François Champollion", C("Howard Carter"), "Flinders Petrie", "Lord Nelson"),
                    Q("Which stone helped people finally read hieroglyphs?",
                        C("The Rosetta Stone"), "The Blarney Stone", "The Moonstone", "The Philosopher's Stone"),
                    Q("What did the Egyptians keep a mummy's organs in?",
                        "Clay bowls", C("Canopic jars"), "Wooden boxes", "Golden cups"),
                    Q("What do we call the kings of ancient Egypt?", C("Pharaohs"), "Emperors", "Sultans", "Tsars"),
                ],
                Comments =
                [
                    By(EirikId, "2026-03-16 19:25", "Rosetta stone and philosopher's stone in the same question haha"),
                    By(SigridId, "2026-03-16 20:10",
                        "One of my pupils picked the Philosopher's Stone and defended it for ten minutes."),
                ],
            },
            new()
            {
                Title = "Norse mythology",
                Description =
                    "Odin, Thor, Loki and the rest. Not the Marvel versions please! "
                    + "And yes there is a question about the tree, I had to.",
                Difficulty = Difficulty.Normal,
                OwnerId = EirikId,
                CreatedAt = On("2026-03-19 19:05"),
                Categories = In(HistoryId, BooksId),
                Questions =
                [
                    Q("What kind of tree is Yggdrasil?", "Oak", C("Ash"), "Birch", "Pine"),
                    Q("What is the name of Odin's horse with eight legs?",
                        C("Sleipnir"), "Fenrir", "Gullinbursti", "Grani"),
                    Q("What are Odin's two ravens called?",
                        "Geri and Freki", C("Huginn and Muninn"), "Skoll and Hati", "Tanngrisnir and Tanngnjóstr"),
                    Q("What is Thor's hammer called?", "Gungnir", C("Mjølnir"), "Draupnir", "Gram"),
                    Q("What animals pull Thor's chariot?", C("Goats"), "Wolves", "Cats", "Horses"),
                    Q("Where do warriors who die in battle go? (one of the places)",
                        "Midgard", C("Valhalla"), "Jotunheim", "Alfheim"),
                    Q("Which day of the week is named after Thor?", "Tuesday", "Wednesday", C("Thursday"), "Friday"),
                ],
                Comments =
                [
                    By(NoraId, "2026-03-19 22:40",
                        "The Thursday question is lovely. Wednesday is Odin's, if anyone's wondering. Woden's day."),
                    By(KasperId, "2026-03-20 01:15", "god of war carried me through this one ngl"),
                    By(JonasId, "2026-03-20 07:30", "Freyja's cats as a wrong answer. Well done."),
                ],
            },
            new()
            {
                Title = "Coffee shop menu, decoded",
                Description =
                    "I work in a café on weekends and people order things they can't explain to me. This one's for them.",
                Difficulty = Difficulty.Normal,
                OwnerId = AlvaId,
                CreatedAt = On("2026-03-23 08:40"),
                Categories = In(FoodAndDrinkId),
                Questions =
                [
                    Q("What does 'latte' mean in Italian?", "Coffee", C("Milk"), "Foam", "Cup"),
                    Q("A macchiato is espresso 'stained' with...",
                        C("A little milk foam"), "Chocolate", "Caramel syrup", "Whipped cream"),
                    Q("The cappuccino is named after...",
                        "A town in Italy", C("Capuchin friars"), "Its inventor, Mr. Cappucci", "A type of cup"),
                    Q("Which bean usually has MORE caffeine?", "Arabica", C("Robusta")),
                    Q("Which two countries argue about who invented the flat white?",
                        "Italy and France", C("Australia and New Zealand"), "UK and Ireland", "USA and Canada"),
                    Q("An affogato is espresso poured over...", "Milk", "Ice cubes", C("Ice cream"), "Whipped cream"),
                    Q("Which country drinks the most coffee per person?", "Italy", "USA", C("Finland"), "Brazil"),
                ],
                Comments =
                [
                    By(EirikId, "2026-03-23 11:15", "Finland! We are close behind I think. Coffee is serious business here"),
                    By(NoraId, "2026-03-23 21:40",
                        "The flat white question is going to start a fight in the comments and I'm here for it."),
                    By(TobbenId, "2026-03-24 08:02", "i just order 'a normal coffee' and hope for the best"),
                ],
            },
            new()
            {
                Title = "Board games",
                Description = "Cardboard only.",
                Difficulty = Difficulty.Normal,
                OwnerId = JonasId,
                CreatedAt = On("2026-03-26 18:20"),
                Categories = In(GamesId),
                Questions =
                [
                    Q("Most expensive property on the original UK Monopoly board?",
                        "Park Lane", C("Mayfair"), "Bond Street", "Oxford Street"),
                    Q("Which chess piece moves in an L shape?", "Bishop", "Rook", C("Knight"), "Queen"),
                    Q("Which of these is not a resource in Catan?", "Ore", "Wool", "Brick", C("Gold")),
                    Q("Points for a Q in English Scrabble?", "5", "8", C("10"), "12"),
                    Q("Number of weapons in classic Cluedo?", "5", C("6"), "7", "9"),
                    Q("Squares on a chessboard?", "48", C("64"), "72", "81"),
                    Q("In Ticket to Ride you are building...", C("Train routes"), "Airports", "Roads", "Canals"),
                ],
            },
            new()
            {
                Title = "zelda (no story stuff)",
                Description =
                    "no story spoilers, just general zelda stuff. and yes the guy is called link. say it with me",
                Difficulty = Difficulty.Normal,
                OwnerId = KasperId,
                CreatedAt = On("2026-03-30 23:05"),
                Categories = In(GamesId),
                Questions =
                [
                    Q("what's the name of the guy you play as", "Zelda", C("Link"), "Ganon", "Epona"),
                    Q("first legend of zelda came out in japan in...", "1984", C("1986"), "1988", "1991"),
                    Q("the triforce is power, wisdom and...", "Strength", C("Courage"), "Hope", "Time"),
                    Q("the kingdom most of the games take place in", C("Hyrule"), "Termina", "Koholint", "Lorule"),
                    Q("breath of the wild came out on switch and which other console", "3DS", C("Wii U"), "Wii", "PS4"),
                    Q("link's horse is called", "Shadowfax", "Agro", C("Epona"), "Roach"),
                    Q("who wrote most of the classic zelda music",
                        "Nobuo Uematsu", C("Koji Kondo"), "Yoko Shimomura", "Grant Kirkhope"),
                ],
                Comments =
                [
                    By(NoraId, "2026-03-31 09:12", "Shadowfax as a wrong answer. I see you."),
                    By(TobbenId, "2026-03-31 17:45", "i honestly thought the guy was zelda until last year"),
                    By(KasperId, "2026-03-31 18:01", "this quiz was made for you specifically"),
                ],
            },
            new()
            {
                Title = "Who said it? (history edition)",
                Description =
                    "Famous lines and who said them. A couple of these are more legend than record, "
                    + "so I've gone with whoever they're usually attributed to.",
                Difficulty = Difficulty.Normal,
                OwnerId = NoraId,
                CreatedAt = On("2026-04-03 20:50"),
                Categories = In(HistoryId),
                Questions =
                [
                    Q("\"I came, I saw, I conquered.\"",
                        C("Julius Caesar"), "Alexander the Great", "Napoleon", "Augustus"),
                    Q("\"Ich bin ein Berliner.\"",
                        "Ronald Reagan", C("John F. Kennedy"), "Willy Brandt", "Richard Nixon"),
                    Q("\"The only thing we have to fear is fear itself.\"",
                        "Winston Churchill", C("Franklin D. Roosevelt"), "Abraham Lincoln", "Theodore Roosevelt"),
                    Q("\"Mr. Gorbachev, tear down this wall!\"",
                        C("Ronald Reagan"), "George H. W. Bush", "Margaret Thatcher", "Helmut Kohl"),
                    Q("\"Let them eat cake.\"",
                        "Catherine the Great", C("Marie Antoinette"), "Louis XVI", "Queen Victoria"),
                    Q("\"I have nothing to offer but blood, toil, tears and sweat.\"",
                        "Franklin D. Roosevelt", "Charles de Gaulle", C("Winston Churchill"), "Clement Attlee"),
                    Q("\"Give me liberty, or give me death!\"",
                        "Thomas Jefferson", "George Washington", "Benjamin Franklin", C("Patrick Henry")),
                ],
                Comments =
                [
                    By(JonasId, "2026-04-04 07:40",
                        "Marie Antoinette almost certainly never said it. Rousseau has a version of it in Confessions, "
                        + "written before she came to France."),
                    By(NoraId, "2026-04-04 09:15", "Hence the weaselly description! But yes, completely agree."),
                    By(EirikId, "2026-04-05 15:20", "Got 6. The Patrick Henry one I did not know"),
                ],
            },
            new()
            {
                Title = "friday pub quiz #4",
                Description = "same as every friday. bit of everything, loser buys the next round",
                Difficulty = Difficulty.Normal,
                OwnerId = TobbenId,
                CreatedAt = On("2026-04-10 17:55"),
                Categories = In(UncategorizedId),
                Questions =
                [
                    Q("how many players in a rugby union team", "11", "13", C("15"), "18"),
                    Q("which country gave the statue of liberty to the usa", C("France"), "UK", "Spain", "Netherlands"),
                    Q("capital of iceland", "Oslo", C("Reykjavík"), "Akureyri", "Tórshavn"),
                    Q("how many strings on a normal guitar", "4", C("6"), "7", "12"),
                    Q("which planet has the most moons", "Jupiter", C("Saturn"), "Uranus", "Neptune"),
                    Q("who painted the mona lisa", C("Leonardo da Vinci"), "Michelangelo", "Raphael", "Botticelli"),
                    Q("how many minutes in a day", "1240", "1400", C("1440"), "1460"),
                    Q("smallest country in the world", C("Vatican City"), "Monaco", "San Marino", "Liechtenstein"),
                ],
                Comments =
                [
                    By(EirikId, "2026-04-10 22:30", "So who bought the round?"),
                    By(TobbenId, "2026-04-11 10:12", "me. again. i wrote the quiz and still lost"),
                    By(AdminId, "2026-04-11 12:00",
                        "Tip: if you add a category or two it'll show up when people filter. "
                        + "Otherwise it just lands in Uncategorized."),
                    By(TobbenId, "2026-04-11 12:20", "its a pub quiz, its every category. leaving it"),
                    By(AlvaId, "2026-04-12 20:33", "1240 minutes was SO tempting"),
                ],
            },
            new()
            {
                Title = "What's in it?",
                Description = "Guess the main ingredient. I made this one with my nephew and he picked the dishes.",
                Difficulty = Difficulty.Easy,
                OwnerId = SigridId,
                CreatedAt = On("2026-04-14 16:40"),
                Categories = In(FoodAndDrinkId),
                Questions =
                [
                    Q("Guacamole is mostly made from...", "Peas", C("Avocado"), "Cucumber", "Green pepper"),
                    Q("Hummus is made from...", C("Chickpeas"), "Lentils", "White beans", "Peanuts"),
                    Q("Which herb is the star of a classic pesto?", "Parsley", "Mint", C("Basil"), "Coriander"),
                    Q("Tzatziki is yoghurt with garlic and...", "Tomato", C("Cucumber"), "Onion", "Spinach"),
                    Q("A Spanish tortilla is made with...",
                        C("Eggs and potatoes"), "Corn flour", "Rice and beans", "Bread and cheese"),
                    Q("Kimchi is usually made from fermented...", "Carrots", "Tofu", C("Cabbage"), "Seaweed"),
                    Q("Where does brunost (Norwegian brown cheese) get its colour from?",
                        "Chocolate", C("Boiling the whey until the sugar caramelises"), "Smoking it", "Brown cows' milk"),
                ],
                Comments =
                [
                    By(EirikId, "2026-04-14 19:00", "Brunost is the best thing Norway ever made. Fight me"),
                    By(NoraId, "2026-04-15 08:30", "'Brown cows' milk' is the best wrong answer on this entire site."),
                    By(SigridId, "2026-04-15 16:12", "That was all my nephew! He was very proud of it."),
                ],
            },
            new()
            {
                Title = "Rivers and mountains",
                Description = "Physical geography. No capitals this time.",
                Difficulty = Difficulty.Hard,
                OwnerId = JonasId,
                CreatedAt = On("2026-04-18 09:10"),
                Categories = In(GeographyId, NatureId),
                Questions =
                [
                    Q("Which river flows through Baghdad?", "Euphrates", C("Tigris"), "Jordan", "Nile"),
                    Q("The Danube runs through four capitals. Which of these is not one of them?",
                        "Vienna", "Budapest", "Belgrade", C("Prague")),
                    Q("Mount Kilimanjaro is in...", "Kenya", C("Tanzania"), "Uganda", "Ethiopia"),
                    Q("Second-highest mountain on Earth?", "Kangchenjunga", C("K2"), "Lhotse", "Makalu"),
                    Q("Deepest lake in the world?",
                        C("Lake Baikal"), "Lake Tanganyika", "Lake Superior", "Caspian Sea"),
                    Q("Angel Falls, the highest uninterrupted waterfall, is in...",
                        "Brazil", "Colombia", C("Venezuela"), "Guyana"),
                    Q("Which range is usually taken as the border between Europe and Asia in Russia?",
                        C("The Urals"), "The Caucasus", "The Carpathians", "The Altai"),
                    Q("Which river carries the most water?", C("Amazon"), "Nile", "Congo", "Yangtze"),
                ],
                Comments =
                [
                    By(NoraId, "2026-04-18 20:05", "Asking about water instead of length was smart. That argument never ends."),
                    By(JonasId, "2026-04-18 20:31", "That was the reason."),
                    By(KasperId, "2026-04-19 23:44", "prague on the danube felt so right"),
                ],
            },
            new()
            {
                Title = "Star Wars, spoiler free (really)",
                Description = "My boyfriend still hasn't seen them, so there's zero plot in here. Promise.",
                Difficulty = Difficulty.Normal,
                OwnerId = AlvaId,
                CreatedAt = On("2026-04-22 20:30"),
                Categories = In(MoviesId, PopCultureId),
                Questions =
                [
                    Q("What's Han Solo's ship called?", C("Millennium Falcon"), "Star Destroyer", "X-wing", "Slave I"),
                    Q("What species is Chewbacca?", "Ewok", C("Wookiee"), "Jawa", "Hutt"),
                    Q("Who wrote the music?", "Hans Zimmer", "Howard Shore", C("John Williams"), "Danny Elfman"),
                    Q("When did the first film come out?", "1975", C("1977"), "1980", "1983"),
                    Q("R2-D2's anxious gold friend is...", "BB-8", C("C-3PO"), "K-2SO", "IG-88"),
                    Q("Luke grows up on a desert planet called...", C("Tatooine"), "Hoth", "Endor", "Naboo"),
                    Q("Han says the Falcon made the Kessel Run in less than how many parsecs?", "10", C("12"), "14", "20"),
                ],
                Comments =
                [
                    By(KasperId, "2026-04-23 00:10", "a parsec is a unit of distance and i will die on this hill"),
                    By(NoraId, "2026-04-23 08:55",
                        "Respect for keeping it clean. Your boyfriend can now take this quiz and still know nothing."),
                    By(AlvaId, "2026-04-26 21:15", "UPDATE: he got 2/7 and is now watching them!! it worked"),
                ],
            },
            new()
            {
                Title = "Winter sports",
                Description =
                    "Skiing, biathlon and some other things on snow and ice. Norway is overrepresented and I am not sorry.",
                Difficulty = Difficulty.Hard,
                OwnerId = EirikId,
                CreatedAt = On("2026-04-27 12:15"),
                Categories = In(SportsId),
                Questions =
                [
                    Q("Which Norwegian city had the Winter Olympics in 1994?",
                        "Oslo", C("Lillehammer"), "Trondheim", "Hamar"),
                    Q("Oslo also had the Winter Olympics once. Which year?", "1948", C("1952"), "1956", "1964"),
                    Q("In biathlon you ski and...", C("Shoot"), "Jump", "Skate", "Snowboard"),
                    Q("Marit Bjørgen won most of her medals in...",
                        "Biathlon", C("Cross-country skiing"), "Alpine skiing", "Speed skating"),
                    Q("In curling, what do you call the target?", "The bullseye", C("The house"), "The circle", "The pot"),
                    Q("Which country has won the most Winter Olympic medals in total?",
                        C("Norway"), "USA", "Germany", "Russia"),
                    Q("Holmenkollen in Oslo is famous for its...",
                        "Football stadium", C("Ski jump"), "Ice hockey arena", "Bobsleigh track"),
                ],
                Comments =
                [
                    By(TobbenId, "2026-04-27 18:40", "norway having the most winter medals is just unfair at this point"),
                    By(SigridId, "2026-04-28 14:05",
                        "My grandmother used to talk about watching the skating at Bislett in '52. She'd have liked this."),
                    By(JonasId, "2026-04-29 07:10", "Hamar is a good wrong answer. The skating was held there in 94."),
                ],
            },
            new()
            {
                Title = "Doctor Who, gently",
                Description =
                    "Sixty-odd years of television, and I've kept it to the basics everyone picks up by osmosis. "
                    + "Nothing beyond the premise.",
                Difficulty = Difficulty.Normal,
                OwnerId = NoraId,
                CreatedAt = On("2026-05-02 21:25"),
                Categories = In(TvShowsId),
                Questions =
                [
                    Q("What does TARDIS stand for?",
                        C("Time And Relative Dimension In Space"), "Travel Across Realities, Dimensions and Spacetime",
                        "Temporal And Relative Distance In Space", "Time Agency Rapid Deployment Interstellar Ship"),
                    Q("What does the TARDIS look like from the outside?",
                        "A red phone box", C("A blue police box"), "A green postbox", "A black taxi"),
                    Q("What is the Doctor's home planet?", "Skaro", "Mondas", C("Gallifrey"), "Telos"),
                    Q("What does the Doctor carry instead of a weapon?",
                        C("A sonic screwdriver"), "A laser pen", "A pocket watch", "A psychic hammer"),
                    Q("When did Doctor Who first air?", "1959", C("1963"), "1968", "1972"),
                    Q("Which enemies are famous for shouting 'Exterminate!'?",
                        "The Cybermen", C("The Daleks"), "The Sontarans", "The Weeping Angels"),
                ],
                Comments =
                [
                    By(AlvaId, "2026-05-03 10:30", "I've never seen a single episode and got 4. osmosis is real"),
                    By(JonasId, "2026-05-03 12:00", "TARDIS is sometimes 'Dimensions', plural. Both should count."),
                    By(NoraId, "2026-05-03 12:45", "Both count. I'm not a monster."),
                ],
            },
            new()
            {
                Title = "fps history",
                Description = "doom to now, mostly pc. i'm bad at all of these games and i love them anyway",
                Difficulty = Difficulty.Hard,
                OwnerId = KasperId,
                CreatedAt = On("2026-05-06 00:50"),
                Categories = In(GamesId),
                Questions =
                [
                    Q("which studio made doom (1993)", C("id Software"), "Valve", "Apogee", "Bungie"),
                    Q("counter-strike started as a mod for", "Quake", C("Half-Life"), "Unreal Tournament", "Team Fortress Classic"),
                    Q("half-life 2 ran on which engine", "GoldSrc", C("Source"), "id Tech 4", "Unreal Engine 2"),
                    Q("halo: combat evolved launched on", C("Xbox"), "PlayStation 2", "GameCube", "Dreamcast"),
                    Q("name of the guy you play in wolfenstein 3d",
                        "Duke Nukem", "Doomguy", C("B.J. Blazkowicz"), "Gordon Freeman"),
                    Q("who makes valorant", "Valve", C("Riot Games"), "Blizzard", "Epic Games"),
                    Q("what year did the original counter-strike mod come out", "1997", C("1999"), "2001", "2003"),
                ],
                Comments =
                [
                    By(EirikId, "2026-05-06 17:25", "Doom on my father's PC. Best memories"),
                    By(TobbenId, "2026-05-07 21:10", "valorant counts as history? feels like it came out yesterday"),
                    By(KasperId, "2026-05-07 21:16", "it came out in 2020 my guy"),
                ],
            },
            new()
            {
                Title = "Pixar for the whole family",
                Description = "A friendly one for movie night. Ask the kids, they'll know!",
                Difficulty = Difficulty.Easy,
                OwnerId = SigridId,
                CreatedAt = On("2026-05-11 17:00"),
                Categories = In(MoviesId),
                Questions =
                [
                    Q("What was Pixar's first full-length film?",
                        C("Toy Story"), "A Bug's Life", "Finding Nemo", "Monsters, Inc."),
                    Q("In Finding Nemo, what kind of fish is Nemo?", "Goldfish", C("Clownfish"), "Pufferfish", "Angelfish"),
                    Q("Which city is Ratatouille set in?", "Rome", "Lyon", C("Paris"), "Marseille"),
                    Q("In Up, how does Carl make his house fly?",
                        C("With lots and lots of balloons"), "With a rocket", "With a giant kite", "With a propeller"),
                    Q("What is the name of the robot WALL-E falls for?", "EVA", C("EVE"), "AVA", "IVY"),
                    Q("How many emotions live in Riley's head in the first Inside Out?", "4", C("5"), "6", "7"),
                    Q("What is the name of the jumping lamp in Pixar's logo?", "Lumi", C("Luxo Jr."), "Lampy", "Pixie"),
                ],
            },
            new()
            {
                Title = "Eurovision!!",
                Description =
                    "Still recovering from this year's final so here's a quiz about all the years before it. "
                    + "No wind machine included, sorry.",
                Difficulty = Difficulty.Normal,
                OwnerId = AlvaId,
                CreatedAt = On("2026-05-17 11:30"),
                // The admin added TV Shows the next morning; see the comments.
                UpdatedAt = On("2026-05-18 09:20"),
                Categories = In(MusicId, PopCultureId, TvShowsId),
                Questions =
                [
                    Q("Which band won for Finland in 2006 wearing monster masks?",
                        C("Lordi"), "Nightwish", "HIM", "The Rasmus"),
                    Q("Who won twice for Sweden, in 2012 and 2023?",
                        "Måns Zelmerlöw", C("Loreen"), "Carola", "Charlotte Perrelli"),
                    Q("Céline Dion won Eurovision for which country??", "Canada", "France", C("Switzerland"), "Belgium"),
                    Q("Måneskin won in 2021 for...", C("Italy"), "Spain", "Portugal", "San Marino"),
                    Q("Which country from outside Europe has been competing since 2015?",
                        "Canada", C("Australia"), "USA", "Japan"),
                    Q("Norway's famous 'nul points' in 1978 was...",
                        C("Jahn Teigen"), "Alexander Rybak", "Bobbysocks", "Wenche Myhre"),
                    Q("What did Alexander Rybak play on stage in 2009?", "Guitar", C("Violin"), "Piano", "Accordion"),
                ],
                Comments =
                [
                    By(EirikId, "2026-05-17 13:02", "Jahn Teigen is a national hero, nul points or not"),
                    By(TobbenId, "2026-05-17 15:40", "lordi is the best thing that ever happened to that contest"),
                    By(NoraId, "2026-05-17 20:15", "I respect any quiz with 'monster masks' in the first question."),
                    By(AdminId, "2026-05-18 09:20",
                        "Added TV Shows as a category on this one, since it is a TV show after all. Hope that's ok, alva."),
                    By(AlvaId, "2026-05-18 12:01", "totally ok!! didn't even think of that"),
                ],
            },
            new()
            {
                Title = "Who sang the sitcom theme?",
                Description =
                    "You've heard all of these hundreds of times and almost certainly never checked who was singing. "
                    + "I hadn't either, until last week.",
                Difficulty = Difficulty.Expert,
                OwnerId = NoraId,
                CreatedAt = On("2026-05-20 22:30"),
                Categories = In(TvShowsId, MusicId),
                Questions =
                [
                    Q("Friends: \"I'll Be There for You\"",
                        "The Lemonheads", C("The Rembrandts"), "Hootie & the Blowfish", "Gin Blossoms"),
                    Q("The Big Bang Theory",
                        C("Barenaked Ladies"), "They Might Be Giants", "Weezer", "Ben Folds Five"),
                    Q("Malcolm in the Middle: \"Boss of Me\"",
                        "Barenaked Ladies", C("They Might Be Giants"), "Fountains of Wayne", "Cake"),
                    Q("Scrubs: \"Superman\"", "Five for Fighting", "Wheatus", C("Lazlo Bane"), "Smash Mouth"),
                    Q("Cheers: \"Where Everybody Knows Your Name\"",
                        "Billy Joel", C("Gary Portnoy"), "Randy Newman", "Harry Nilsson"),
                    Q("How I Met Your Mother: \"Hey Beautiful\"", "The Fray", "OK Go", C("The Solids"), "The Format"),
                    Q("Gilmore Girls: \"Where You Lead\"",
                        C("Carole King"), "Joni Mitchell", "Carly Simon", "Linda Ronstadt"),
                ],
                Comments =
                [
                    By(AlvaId, "2026-05-21 08:40", "Lazlo Bane?? I've been singing that song for 20 years and never once wondered"),
                    By(JonasId, "2026-05-21 09:02", "Five for Fighting on the Scrubs question is a mean trick."),
                    By(KasperId, "2026-05-22 00:30", "wait the solids are real?? i thought they were made up for the show"),
                    By(NoraId, "2026-05-22 08:14", "Real band! The two people who created the show are in it."),
                ],
            },
            new()
            {
                Title = "Harry Potter, but no plot",
                Description =
                    "Nothing about what actually happens!! Just houses, sweets, platforms etc. "
                    + "Safe if you've only seen the first film.",
                Difficulty = Difficulty.Easy,
                OwnerId = AlvaId,
                CreatedAt = On("2026-05-23 19:20"),
                Categories = In(BooksId, MoviesId),
                Questions =
                [
                    Q("Which platform does the Hogwarts Express leave from?",
                        "Platform 7 1/2", C("Platform 9 3/4"), "Platform 10 1/3", "Platform 12"),
                    Q("How many houses are there at Hogwarts?", "3", C("4"), "5", "6"),
                    Q("What's Harry's owl called?", "Errol", "Pigwidgeon", C("Hedwig"), "Hermes"),
                    Q("In Quidditch, which ball ends the game when someone catches it?",
                        "The Quaffle", "A Bludger", C("The Golden Snitch")),
                    Q("What do wizards in Britain call non-magic people?", C("Muggles"), "No-Majs", "Squibs", "Plainfolk"),
                    Q("What's the wizarding shopping street in London called?",
                        C("Diagon Alley"), "Privet Drive", "Hogsmeade", "Godric's Hollow"),
                    Q("Which sweet might hop out the window if you're too slow?",
                        "A Bertie Bott's bean", C("A Chocolate Frog"), "A Fizzing Whizzbee", "A Pumpkin Pasty"),
                ],
                Comments =
                [
                    By(SigridId, "2026-05-24 09:10", "Lovely and safe! My daughter is on book two and got six."),
                    By(NoraId, "2026-05-24 11:42", "No-Majs as a wrong answer on an Easy quiz is a bold move."),
                    By(AlvaId, "2026-05-24 12:00", "had to put SOMETHING hard in there"),
                ],
            },
            new()
            {
                Title = "the office (us)",
                Description =
                    "rewatching it for the 6th time. nothing past the first couple of seasons, mostly just the setup",
                Difficulty = Difficulty.Normal,
                OwnerId = KasperId,
                CreatedAt = On("2026-05-26 23:15"),
                Categories = In(TvShowsId),
                Questions =
                [
                    Q("name of the paper company",
                        C("Dunder Mifflin"), "Wernham Hogg", "Vance Refrigeration", "Paper Plus"),
                    Q("which city is the branch in", "Stamford", C("Scranton"), "Utica", "Nashua"),
                    Q("what kind of farm does dwight have", C("beets"), "potatoes", "dairy", "turkeys"),
                    Q("what does jim put dwight's stapler in", "the fridge", C("jell-o"), "a bucket of paint", "the shredder"),
                    Q("the uk original was made by ricky gervais and",
                        C("Stephen Merchant"), "Karl Pilkington", "Martin Freeman", "Simon Pegg"),
                    Q("what does michael's mug say",
                        "Best Boss Ever", C("World's Best Boss"), "#1 Manager", "Boss of the Year"),
                    Q("who plays michael scott", C("Steve Carell"), "Rainn Wilson", "John Krasinski", "Ed Helms"),
                ],
                Comments =
                [
                    By(TobbenId, "2026-05-27 18:20", "wernham hogg as a wrong answer, ok you're a real fan"),
                    By(AlvaId, "2026-05-27 20:05", "the jell-o stapler will never not be funny"),
                    By(EirikId, "2026-05-28 07:50", "I have only seen the UK one. 3/7"),
                ],
            },
            new()
            {
                Title = "rap classics",
                Description = "mostly 90s because that's what my brother played in the car every single day",
                Difficulty = Difficulty.Hard,
                OwnerId = TobbenId,
                CreatedAt = On("2026-05-29 22:00"),
                Categories = In(MusicId),
                Questions =
                [
                    Q("nas debut album (1994)", C("Illmatic"), "It Was Written", "Ready to Die", "Reasonable Doubt"),
                    Q("which kendrick album won the pulitzer",
                        "good kid, m.A.A.d city", "To Pimp a Butterfly", C("DAMN."), "Mr. Morale & the Big Steppers"),
                    Q("who put out the chronic in 1992", "Ice Cube", C("Dr. Dre"), "Snoop Dogg", "Eazy-E"),
                    Q("wu-tang's first album is enter the wu-tang (__ chambers)", "12", "18", C("36"), "64"),
                    Q("outkast are from", "Houston", "Memphis", C("Atlanta"), "New Orleans"),
                    Q("first rap song to win the oscar for best original song",
                        C("Lose Yourself"), "It's Hard Out Here for a Pimp", "Glory", "Shallow"),
                    Q("lauryn hill's only solo studio album",
                        "The Score", C("The Miseducation of Lauryn Hill"), "Unplugged", "Blunted on Reality"),
                ],
                Comments =
                [
                    By(KasperId, "2026-05-30 01:05", "36 chambers i should have known. embarrassing"),
                    By(JonasId, "2026-05-30 07:45",
                        "The Oscar question is good. Three 6 Mafia as an option would have been nastier."),
                    By(AlvaId, "2026-05-30 19:20", "my brother played the exact same stuff!! got 5"),
                ],
            },
            new()
            {
                Title = "Vikings (the real ones)",
                Description = "Real vikings, not the TV show. Some is easy, some not.",
                Difficulty = Difficulty.Normal,
                OwnerId = EirikId,
                CreatedAt = On("2026-06-09 13:40"),
                Categories = In(HistoryId),
                Questions =
                [
                    Q("Which raid in 793 is often called the start of the Viking Age?",
                        C("Lindisfarne"), "Iona", "Canterbury", "Whitby"),
                    Q("Did vikings wear helmets with horns in battle?",
                        "Yes, always", "Only the chiefs", C("No, that came much later")),
                    Q("Who is thought to be the first European to land in North America?",
                        "Erik the Red", C("Leif Erikson"), "Christopher Columbus", "Harald Hardrada"),
                    Q("The Norse site L'Anse aux Meadows is in...",
                        "Nova Scotia", C("Newfoundland"), "Quebec", "British Columbia"),
                    Q("What is the runic alphabet called?", C("Futhark"), "Ogham", "Cyrillic", "Glagolitic"),
                    Q("The Oseberg ship was found in...", C("Norway"), "Denmark", "Sweden", "Iceland"),
                    Q("Harald Hardrada died in 1066 at the battle of...",
                        "Hastings", C("Stamford Bridge"), "Maldon", "Clontarf"),
                ],
                Comments =
                [
                    By(JonasId, "2026-06-09 18:00", "The helmet question should be required in every Viking quiz."),
                    By(TobbenId, "2026-06-10 12:30", "stamford bridge in 1066, chelsea have been around a long time"),
                    By(NoraId, "2026-06-11 20:40", "Saw the Oseberg ship on a school trip and still think about it."),
                ],
            },
            new()
            {
                Title = "Cheese, mostly European",
                Description =
                    "Match the cheese to where it comes from, plus a few questions about how it's made. "
                    + "I ate a lot of cheese researching this. No regrets.",
                Difficulty = Difficulty.Hard,
                OwnerId = NoraId,
                CreatedAt = On("2026-06-21 19:05"),
                Categories = In(FoodAndDrinkId, GeographyId),
                Questions =
                [
                    Q("Parmigiano-Reggiano comes from...", C("Italy"), "France", "Switzerland", "Spain"),
                    Q("Roquefort is made from which milk?", "Cow's", "Goat's", C("Sheep's"), "Buffalo"),
                    Q("Halloumi is traditionally from...", "Greece", "Turkey", C("Cyprus"), "Lebanon"),
                    Q("Gouda comes from...", "Belgium", "Germany", C("The Netherlands"), "Denmark"),
                    Q("Stilton is a blue cheese from...", C("England"), "Scotland", "Ireland", "Wales"),
                    Q("What makes the holes in Emmental?",
                        "Air pumped in while it's pressed", C("Gas from bacteria"), "Mice, historically",
                        "Salt crystals dissolving"),
                    Q("Proper mozzarella di bufala is made from the milk of...",
                        "Cows", C("Water buffalo"), "Goats", "Sheep"),
                    Q("Only one country is allowed to call its cheese feta in the EU. Which?",
                        "Bulgaria", "Turkey", "Denmark", C("Greece")),
                ],
                Comments =
                [
                    By(EirikId, "2026-06-21 21:12", "No brunost?? I am disappointed"),
                    By(NoraId, "2026-06-21 21:40",
                        "Some people argue brunost isn't technically cheese, and I didn't want to start that fight "
                        + "in my own comments."),
                    By(AlvaId, "2026-06-22 18:15", "the mice answer made me laugh way too much"),
                    By(SigridId, "2026-06-24 15:30", "Learned the Emmental one today. Thank you!"),
                ],
            },
            new()
            {
                Title = "Physics",
                Description = "Units and constants, mostly. A few names.",
                Difficulty = Difficulty.Expert,
                OwnerId = JonasId,
                CreatedAt = On("2026-07-04 06:40"),
                Categories = In(ScienceId),
                Questions =
                [
                    Q("Speed of light in a vacuum, roughly?",
                        "186,000 km/s", C("300,000 km/s"), "150,000 km/s", "3,000,000 km/s"),
                    Q("SI unit of force?", "Joule", C("Newton"), "Pascal", "Watt"),
                    Q("Absolute zero in Celsius?", "-459.67 °C", "-300 °C", C("-273.15 °C"), "-173.15 °C"),
                    Q("Who discovered the electron, in 1897?",
                        "Ernest Rutherford", C("J. J. Thomson"), "Niels Bohr", "James Chadwick"),
                    Q("Year Einstein published special relativity?", "1900", C("1905"), "1915", "1921"),
                    Q("Particle confirmed at CERN in 2012?", "Top quark", "W boson", C("Higgs boson"), "Neutrino"),
                    Q("SI unit of electrical resistance?", "Ampere", "Volt", "Coulomb", C("Ohm")),
                    Q("Which of these is a fermion?", C("Electron"), "Photon", "Gluon", "Higgs boson"),
                ],
                Comments =
                [
                    By(NoraId, "2026-07-04 21:30",
                        "The miles-per-second trap on question one is evil. I walked right into it."),
                    By(KasperId, "2026-07-05 02:14", "3/8 and two of those were guesses"),
                    By(JonasId, "2026-07-05 07:02", "Three is more than most."),
                ],
            },
            new()
            {
                Title = "Birds",
                Description =
                    "I became a birdwatcher this spring, which I'm told happens to everyone eventually. "
                    + "A few of these come from my own notebook; most come from books.",
                Difficulty = Difficulty.Normal,
                OwnerId = NoraId,
                CreatedAt = On("2026-07-19 07:15"),
                Categories = In(NatureId),
                Questions =
                [
                    Q("Which bird is the fastest animal on Earth, in a dive?",
                        C("Peregrine falcon"), "Golden eagle", "Common swift", "Albatross"),
                    Q("Which bird can fly backwards?", "Kingfisher", C("Hummingbird"), "Swallow", "Kestrel"),
                    Q("Which bird has the longest migration?",
                        "Barn swallow", "Bar-tailed godwit", C("Arctic tern"), "White stork"),
                    Q("The kiwi is the national bird of...", "Australia", C("New Zealand"), "South Africa", "Fiji"),
                    Q("A baby swan is called a...", "Gosling", C("Cygnet"), "Squab", "Eyas"),
                    Q("Largest living bird?", C("Ostrich"), "Emu", "Cassowary", "Albatross"),
                    Q("Which of these can't fly?", "Puffin", "Cormorant", C("Penguin"), "Pelican"),
                    Q("Norway's national bird is the...", "Puffin", "Sea eagle", "Magpie", C("White-throated dipper")),
                ],
                Comments =
                [
                    By(EirikId, "2026-07-19 12:40", "The fossekall! Nobody outside Norway knows this one"),
                    By(SigridId, "2026-07-20 09:05", "Cygnet! I knew that one from a picture book I read to my class."),
                    By(JonasId, "2026-07-21 06:58",
                        "The bar-tailed godwit has the longest non-stop flight, for anyone who picked it. Different record."),
                ],
            },
            new()
            {
                Title = "Nordic noir on TV",
                Description = "Dark Scandinavian crime shows. No spoilers about who did it, only questions about the shows.",
                Difficulty = Difficulty.Hard,
                OwnerId = EirikId,
                CreatedAt = On("2026-08-06 20:35"),
                Categories = In(TvShowsId),
                Questions =
                [
                    Q("The Bridge (Bron/Broen) is about a bridge between which countries?",
                        "Norway and Sweden", C("Sweden and Denmark"), "Denmark and Germany", "Finland and Sweden"),
                    Q("Which show is about a Danish politician who becomes prime minister?",
                        C("Borgen"), "The Killing", "Lilyhammer", "Occupied"),
                    Q("Lilyhammer has which American actor as a gangster living in Norway?",
                        "James Gandolfini", C("Steven Van Zandt"), "Michael Imperioli", "Tony Sirico"),
                    Q("Sarah Lund in The Killing is famous for wearing...",
                        "A red raincoat", C("A Faroese knitted jumper"), "A leather jacket", "A yellow scarf"),
                    Q("In which town does the Swedish detective Kurt Wallander work?",
                        "Malmö", C("Ystad"), "Gothenburg", "Kiruna"),
                    Q("Which Norwegian series imagines Russia occupying Norway?",
                        "Beforeigners", "Ragnarok", C("Occupied"), "Exit"),
                    Q("Who wrote the books the Wallander series is based on?",
                        "Jo Nesbø", C("Henning Mankell"), "Stieg Larsson", "Camilla Läckberg"),
                ],
                Comments =
                [
                    By(NoraId, "2026-08-07 08:20", "The jumper question! That jumper had its own fan club."),
                    By(KasperId, "2026-08-08 23:30", "lilyhammer is so underrated"),
                ],
            },
            new()
            {
                Title = "Christopher Nolan",
                Description = "Production facts only. No endings.",
                Difficulty = Difficulty.Hard,
                OwnerId = JonasId,
                CreatedAt = On("2026-08-14 07:05"),
                Categories = In(MoviesId),
                Questions =
                [
                    Q("Nolan's first feature film?", C("Following"), "Memento", "Insomnia", "Doodlebug"),
                    Q("Memento tells part of its story in which order?",
                        "Chronological", C("Reverse chronological"), "Random", "Real time"),
                    Q("Who played the Joker in The Dark Knight?",
                        "Jared Leto", "Joaquin Phoenix", C("Heath Ledger"), "Jack Nicholson"),
                    Q("Which film won Nolan his first Oscar for Best Director?",
                        "Dunkirk", "Inception", C("Oppenheimer"), "Interstellar"),
                    Q("Who composed the score for Interstellar?",
                        C("Hans Zimmer"), "Ludwig Göransson", "John Williams", "Howard Shore"),
                    Q("Dunkirk is set during...", "World War I", C("World War II"), "The Korean War", "The Napoleonic Wars"),
                    Q("What's unusual about the title Tenet?",
                        "It's an acronym", C("It's a palindrome"), "It's Latin for 'time'", "It's a character's name"),
                    Q("His 2026 film is based on which epic poem?",
                        "The Iliad", C("The Odyssey"), "Beowulf", "The Aeneid"),
                ],
                Comments =
                [
                    By(KasperId, "2026-08-14 22:10", "the tenet palindrome thing blew my mind way more than the actual movie did"),
                    By(TobbenId, "2026-08-15 11:35", "following is a real movie?? never heard of it"),
                    By(NoraId, "2026-08-16 20:00", "'No endings' is the only safe rule for a Nolan quiz."),
                ],
            },
            new()
            {
                Title = "Olympic history",
                Description =
                    "A bit of Olympic history. We're starting the ancient Games with my class this term, "
                    + "so here is the modern half.",
                Difficulty = Difficulty.Normal,
                OwnerId = SigridId,
                CreatedAt = On("2026-09-02 15:30"),
                Categories = In(SportsId, HistoryId),
                Questions =
                [
                    Q("Where were the first modern Olympic Games held, in 1896?", C("Athens"), "Paris", "London", "Rome"),
                    Q("How many rings are on the Olympic flag?", "4", C("5"), "6", "7"),
                    Q("How many gold medals did Jesse Owens win in Berlin in 1936?", "2", "3", C("4"), "5"),
                    Q("Who scored the first perfect 10 in Olympic gymnastics, in 1976?",
                        "Olga Korbut", C("Nadia Comăneci"), "Simone Biles", "Larisa Latynina"),
                    Q("How many Olympic gold medals did Michael Phelps win in total?", "18", "21", C("23"), "28"),
                    Q("Which city will host the 2028 Summer Olympics?", "Brisbane", C("Los Angeles"), "Paris", "Madrid"),
                    Q("Usain Bolt's 100 m world record is...", "9.69", "9.63", C("9.58"), "9.72"),
                ],
                Comments =
                [
                    By(TobbenId, "2026-09-02 20:45", "9.58 vs 9.63 is a trap and i walked straight into it"),
                    By(EirikId, "2026-09-03 18:10", "Good quiz! Winter olympics next time please"),
                    By(SigridId, "2026-09-03 19:00", "There's already a very good one on here, Eirik. Yours!"),
                ],
            },
        ];

        for (var i = 0; i < quizzes.Length; i++)
        {
            Stamp(quizzes[i], number: i + 1);
        }

        return quizzes;
    }

    private static void Stamp(Quiz quiz, int number)
    {
        quiz.Id = SeedId(200 + number);
        if (quiz.UpdatedAt == default)
        {
            quiz.UpdatedAt = quiz.CreatedAt;
        }

        var q = 0;
        foreach (var question in quiz.Questions)
        {
            q++;
            question.Id = SeedId(3_000_000 + number * 1_000 + q);
            question.CreatedAt = quiz.CreatedAt.AddMinutes(q);

            var o = 0;
            foreach (var option in question.AnswerOptions)
            {
                o++;
                option.Id = SeedId(4_000_000 + number * 1_000 + q * 10 + o);
                option.CreatedAt = question.CreatedAt;
            }
        }

        var c = 0;
        foreach (var comment in quiz.Comments)
        {
            c++;
            comment.Id = SeedId(5_000_000 + number * 100 + c);
        }
    }

    private static Guid SeedId(int suffix) => new($"0a1b7f2c-0000-4000-8000-{suffix:D12}");

    private static Question Q(string text, params Opt[] options) =>
        new()
        {
            Text = text,
            AnswerOptions = [.. options.Select(o => new AnswerOption { Text = o.Text, IsCorrect = o.IsCorrect })],
        };

    private static Opt C(string text) => new(text, IsCorrect: true);

    private static Comment By(Guid authorId, string at, string body, string? edited = null) =>
        new()
        {
            AuthorId = authorId,
            Body = body,
            CreatedAt = On(at),
            UpdatedAt = On(edited ?? at),
        };

    /// <summary>An answer option; a plain string is a wrong answer, <see cref="C"/> marks the right one.</summary>
    private readonly record struct Opt(string Text, bool IsCorrect)
    {
        public static implicit operator Opt(string text) => new(text, IsCorrect: false);
    }
}
