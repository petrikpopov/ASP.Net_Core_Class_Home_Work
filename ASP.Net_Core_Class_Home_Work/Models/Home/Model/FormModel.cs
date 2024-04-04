using Microsoft.AspNetCore.Mvc;

namespace ASP_.Net_Core_Class_Home_Work.Models.Home.Model;
//Модель форми - це відображення данних що приходять з html форми, а також узгодження імен, прийнятих у html (user-name), до імен, традиційних для платформ (UserName)
public class FormModel
{
    [FromForm(Name = "user-name")]
    public string UserName { set; get; } = null!;
    [FromForm(Name = "user-email")]
    public string UserEmail { set; get; } = null!;
}
//HTTP-запит
/*
 POST/Home?Index HTTP/1.1            | POST - метод запиту
 Host:localhost                      | Home/Index - Path
 Connection:close                    | HTTP/1.1 - протокол
 Content-Type:allication/.json       | Заголовки - по одному в рядку
                                     | Порожній рядок - кінець заголовків
 {"name":"User Name" ...}            | Тіло запиту - усе до кінця пакету
 Метод запиту - перше слово у пакеті. Є стандартним
 GET, POST, PUT, DELETE, HEAD, OPTIONS, CONNECT, PATCH
 є промислово-стардартні
 LINK, UNLINK, PURGE
 є інші - як правило сервер сприймає довільні слова "Hello"

 HTTP-відповідь

 HTTP/1.1 200 OK            | 200-статус код
 Connection:close           | OK - reason phrase
  Content-Type:text/html

  <!doctype html>
 */
