import java.util.concurrent.atomic.*;

public class DocumentDB 
{	           
	private static class Embrulho<T>{
		public volatile T _item;
	
		Embrulho(T doc)
		{
			setDoc(doc);
		}
		
		public T getDoc(){
			return _item;
		}
		public void setDoc(T doc)
		{
			_item = doc;
		}
        }
	
	private static class Document 
	{
                private final AtomicInteger _version;
		private final String _text;
		
		public Document(int ver, String txt) 
                { 
                    _text    = txt; 
                    _version = new AtomicInteger(ver);
                    
                }
                
                public int get_Version(){ return _version.get(); }
                public int get_VersionIncrementAndGet(){ return _version.incrementAndGet(); }
                
                public String get_Text(){ return _text; }
                
	}
	
	
        private final AtomicInteger _documentCount;
	private final Document[] _store;
	
	public DocumentDB(int sz) 
	{
           _documentCount = new AtomicInteger( 0 );
	   _store         = new Document[sz];
	}
	
	public int store(String text) 
	{
		Embrulho<Document> emp = new Embrulho<Document>(new Document(0, text));
                int id =_documentCount.getAndIncrement(); 
                _store[ id ] = emp.getDoc();                
		return id; 
	}
	
	public String get(int id) 
	{
            return _store[id].get_Text(); 
	}
	
	public void update(int id, String newText) 
        {
            int versionId = _store[id].get_VersionIncrementAndGet();
            Embrulho<Document> emp = new Embrulho<Document>(new Document(versionId, newText));            		
	}
        
        public static void main( String[] args ){
            System.out.println("Hello world!!!");
        }
}
