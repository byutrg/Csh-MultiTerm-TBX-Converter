using System;

public class Singleton
{
    private static Singleton instance;

	private Singleton() {}

    public static Singleton Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new Singleton();
            }
            return instance;
        }
    }

    private string dialect;
    private string pathToXML;
    private int saveOption;

    public string getDialect()
    {
        return dialect;
    }

    public void setDialect(string d)
    {
        dialect = d;   
    }

    public string getPath()
    {
        return pathToXML;
    }

    public void setPath(string path)
    {
        pathToXML = path;
    }

    public int getSaveOption()
    {
        return saveOption;
    }

    public void setSaveOption(int option)
    {
        saveOption = option;
    }
}
