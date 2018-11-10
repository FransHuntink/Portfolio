import java.io.IOException;
import java.util.ArrayList;
import java.util.List;

import Algorithm.TextAnalyzer;
import Algorithm.TextReader;
import Data.CompanyProfile;
import Data.SaveData;
import StaxUtilities.Feed;
import StaxUtilities.FeedMessage;
import StaxUtilities.RSSFeedParser;
import Utilities.TextFormatting;

public class RSSCrawler {

	/* This is the system class	 */
	private final String CRAWL_URL = "Collection of URLS to crawl through go here";
	private List<CompanyProfile> companyProfiles = new ArrayList<CompanyProfile>();


    /* define lists that will be used by analyzer (input words,
     * signal words and possible companies  */

    private List badWordsList = new ArrayList<String>(); // should move to another class, perhaps its own
    private List companyList = new ArrayList<String>(); // should move to another class, perhaps its own

    /* Declare variables used by the Stax utilities
     *  to read RSS data */
    private  TextReader reader;
    private TextAnalyzer analyzer;
    private RSSFeedParser parser;
    private Feed feed;

    /*  Crawl returns a string with formatted
     * information to its caller. It will also create
     * or update a companyProfile with acquired data */

    public RSSCrawler() throws Exception {

        /* Load previously saved profiles */
        try
        {
            this.LoadProfiles();
            System.out.println("Succesfully loaded profiles!");
        }
        catch(Exception e)
        {
            System.out.println("No profile savefile was loaded on startup");
        }


        /* Create the reader that will parse text from our dataset
         * files */
        reader = new TextReader();

        /* Define the declared lists with data from the dataset using
         * the reader.  */
        badWordsList = reader.FormatString(TextReader.ReadFile("badwords.txt"), true);
        companyList = reader.FormatString(TextReader.ReadFile("companies.txt"), true);

        /* Define the analyzer that will compare
         *  the content in question with our signal words
         *  datasaet */
        analyzer = new TextAnalyzer(badWordsList, companyList);

        /* The code below initializes the Stax RSSFeedParser
         * based on the supplied URL, and then extracts all data from
         * this URL (should be RSS feed) */
        parser = new RSSFeedParser(CRAWL_URL);
        feed = parser.readFeed();


    }


    /* This method crawls through all feeds and returns a
     formatted string that informs the user on what has been
     analyzed. */
	public String Crawl() throws Exception
	{
		int articleIndex = 0;
		String toReturn = "";

		/* We iterate through all feed messages */
		for (FeedMessage message : feed.getMessages())
		{
			articleIndex ++;

			toReturn += String.format("%-30s %s\n", "Article number", articleIndex); 
			toReturn += String.format("%-30s %s\n", "Title:", message.getTitle());

			/* We have a description and title for every
			 article and format it using the TextReader class */

			String description = message.getDescription();
			List descrFormatted = reader.FormatString(description, true);
			String title = message.getTitle();
			List titleFormatted = reader.FormatString(title, true);

			/* We add the analyzer data for the description
			of the article to the return	 */
			toReturn += analyzer.AnalyzeList(descrFormatted);

			/* Analyzer has recognized the company, so we can save the
			analyzer data to either a new or existing company profile */

			if(analyzer.ReturnCompany(titleFormatted) != null)
			{
				String companyName = analyzer.ReturnCompany(titleFormatted);
				toReturn += String.format("%-30s %s\n", "Most likely company: ", analyzer.ReturnCompany(titleFormatted));
				
				/* In case we determine the company name, we
				 * add the score of that company to either
				 * a new or existing company profile */

				AddProfile(companyName, analyzer.ReturnLastScore(), message.getTitle());

			}

			/* We return all the results as a formatted string  */

			toReturn += '\n';
			toReturn += TextFormatting.addLine();
		}
		
		SaveData();
		return toReturn;
	}

	/* Check if profile exists, if so, we add the score 
	 * to that profile. If it doesn't exist, we create one
	 * and then add the score.  */
	
	private void AddProfile(String company, int score, String title)
	{
	    /* Create new instance of CompanyProfile */

        CompanyProfile targetProfile;
        boolean hasEntry;

		if(score == 0)
			return;
			
		for(int i = 0; i < companyProfiles.size(); i++)
        {
			targetProfile = companyProfiles.get(i);

			/* Does the current company have an entry */
			hasEntry = companyProfiles.get(i).CompanyName().equalsIgnoreCase(company);			

			if(hasEntry)
			{
			    /* Article has not yet been analyzed, we add
			    its score to the target company profile */

			    if(!targetProfile.ArticleExists(title))
                {
                    targetProfile.AddScore(score,title);
                    targetProfile.AddArticle(title);
                }

                return;
			}	
		}

		/* No entry found for this profile, create one
		* And then add the score to the new profile */

		CompanyProfile newProfile = new CompanyProfile(company);
		newProfile.AddScore(score,title);
		newProfile.AddArticle(title);
		companyProfiles.add(newProfile);
	}

	/* Save company profile data tos a file */
	private String SaveData()
	{
	    /* We save the data to our local machine */
		return SaveData.SaveProfile(companyProfiles);
	}

	/* Loads profiles from the root directory */
    private void LoadProfiles() throws IOException, ClassNotFoundException
    {
        companyProfiles = SaveData.LoadProfiles();
    }



    /* Returns the scores collected from all profiles
    in a formatted way.
     */
	public String ReturnScores()
	{

	    String scores = "";


		for(int i = 0; i < companyProfiles.size(); i++)
		{
		    CompanyProfile targetProfile = companyProfiles.get(i);

            scores += String.format("%-25s %s\n", "Company name: ", targetProfile.CompanyName());
            scores += String.format("%-25s %s\n", "Negative scores: ", targetProfile.AllScores());

		}
		
		return scores;
	}


}
