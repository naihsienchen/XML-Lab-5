using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using System.Xml;

namespace XML_Lab_5_1.Controllers
{
    public class BookController : Controller
    {
        public IActionResult List()
        {
            IList<Models.Book> bookList = new List<Models.Book>();

            //load books.xml
            //I don't understand this PathBase thing
            string path = Request.PathBase + "App_Data/books.xml";
            XmlDocument doc = new XmlDocument();
            //making sure if file exists before loading it
            if (System.IO.File.Exists(path))
            {
                //access the node list
                doc.Load(path);
                XmlNodeList books = doc.GetElementsByTagName("book");

                //loop through nodes
                foreach (XmlElement b in books)
                {
                    Models.Book book = new Models.Book();
                    book.BookID = b.GetElementsByTagName("id")[0].InnerText;
                    book.Title = b.GetElementsByTagName("title")[0].InnerText;
                    book.AuthorTitle = b.GetAttribute("title");
                    book.FirstName = b.GetElementsByTagName("firstname")[0].InnerText;
                    //an author might not have a middle name
                    if (b.GetElementsByTagName("middlename")[0] != null)
                    {
                        book.MiddleName = b.GetElementsByTagName("middlename")[0].InnerText;
                    }
                    book.LastName = b.GetElementsByTagName("lastname")[0].InnerText;

                    bookList.Add(book);
                }

            }
            //ViewBag.Output = "Everything's connected";
            return View(bookList);
            //how do I redirect to the list page?
        }
        [HttpGet]
        public IActionResult Create()
        {
            var book = new Models.Book();
            return View(book);
            //how do I redirect to the list page?
        }

        [HttpPost]
        public IActionResult Create(Models.Book b)
        {
            //load books.xml
            //I don't understand this PathBase thing
            string path = Request.PathBase + "App_Data/books.xml";
            XmlDocument doc = new XmlDocument();
            //making sure if file exists before loading it
            if (System.IO.File.Exists(path))
            {
                //if file exists, load it and create new book
                doc.Load(path);

                //create a new book
                XmlElement book = _CreateBookElement(doc, b);

                //get the root element
                doc.DocumentElement.AppendChild(book);

            }
            else
            {
                //file doesn't exist: create and create new book
                XmlNode dec = doc.CreateXmlDeclaration("1.0", "uft-8", "");
                doc.AppendChild(dec);
                XmlNode root = doc.CreateElement("books");

                //create a new book
                XmlElement book = _CreateBookElement(doc, b);
                root.AppendChild(book);
                doc.AppendChild(root);
            }
            doc.Save(path);
            return View();
            //how do I redirect to the list page?
        }

        private XmlElement _CreateBookElement(XmlDocument doc, Models.Book newBook)
        {
            XmlElement book = doc.CreateElement("book");
            XmlAttribute authortitle = doc.CreateAttribute("title");
            authortitle.Value = newBook.AuthorTitle;
            book.Attributes.Append(authortitle);

            XmlNode bookid = doc.CreateElement("id");
            bookid.InnerText = newBook.BookID;
            book.AppendChild(bookid);


            XmlNode booktitle = doc.CreateElement("title");
            booktitle.InnerText = newBook.Title;
            book.AppendChild(booktitle);

            XmlNode author = doc.CreateElement("author");
            XmlNode firstname = doc.CreateElement("firstname");
            firstname.InnerText = newBook.FirstName;
            //don't know how to deal with exception (an author without middlename)
            XmlNode middlename = doc.CreateElement("middlename");
            middlename.InnerText = newBook.MiddleName;
            XmlNode lastname = doc.CreateElement("lastname");
            lastname.InnerText = newBook.LastName;
            

            author.AppendChild(firstname);
            author.AppendChild(middlename);
            author.AppendChild(lastname);

            book.AppendChild(author);

            return book;
        }
    }
}
