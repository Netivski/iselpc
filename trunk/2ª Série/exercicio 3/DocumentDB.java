
/**
 * Class whose instances represent in-memory text document databases.
 * 
 * Note: This class instances are not thread-safe.
 */
public class DocumentDB 
{	
	/**
	 * Class whose instances represent in-memory text documents. The class instances
	 * are immutable.  
	 */
	private static class Embrulho{
		public volatile Document item;
	
		Embrulho(Document doc)
		{
			setDoc(doc);
		}
		
		public Document getDoc(){
			return item;
		}
		public void setDoc(Document doc)
		{
			item = doc;
		}
	}
	
	private static class Document 
	{
		/** The document's version number. */
		private final int _version;
		
		/** The document's content. */
		private final String _text;
		
		/**
		 * Initializes an instance with the given contents.
		 * 
		 * @param ver The document's version.
		 * @param txt The document's text.
		 */
		public Document(int ver, String txt) { _version = ver; _text = txt; }
                
                public int get_Version(){ return _version; }
                public String get_Text(){ return _text; }
                
	}
	
	/** The current count of stored documents. */
	private int _documentCount;
	
	/** The array used to store the document's instances. */
	private final Document[] _store;
	
	/**
	 * Initiates an instance with the given capacity.
	 * 
	 * @param sz The maximum number of documents that can be stored. 
	 */
	public DocumentDB(int sz) 
	{
		_documentCount = 0;
	   _store = new Document[sz];
	}
	
	/**
	 * Stores a document with the given content and returns its identifier.
	 * 
	 * @param text The document's content.
	 * @return The document's identifier. 
	 */
	public int store(String text) 
	{
		Embrulho emp = new Embrulho(new Document(0, text));
		_store[ _documentCount++] = emp.getDoc();// new Embrulho(new Document(0, text)).getDoc();
		return _documentCount; 
	}
	
	/**
	 * Gets the content of the document with the given identifier.
	 * 
	 * @param id The document's identifier.
	 * @return The document's text.
	 */
	public String get(int id) 
	{		
		return _store[id].get_Text(); 
	}
	
	/**
	 * Updates the contents of the document with the given identifier.
	 *  
	 * @param id The document's identifier.
	 * @return The document's new text.
	 */
	public void update(int id, String newText) 
	{		
		//_store[id] = new Embrulho(new Document(_store[id]._version + 1, newText));
	}
        
        public static void main( String[] args ){
            System.out.println("Hello world!!!");
        }
}
