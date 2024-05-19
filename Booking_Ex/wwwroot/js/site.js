// Please see documentation at https://docs.microsoft.com/aspnet/core/client-side/bundling-and-minification
// for details on configuring this project to bundle and minify static web assets.
document.addEventListener('submit', e=>{
    console.log(e);
    const form = e.target;
    if(form.id == 'room-form')// перехоплюємо надсилання форми і переводимо до аякс
    {
        e.preventDefault();
        if(!addNewRoom()){
            return;
        }
        let formData = new FormData(form);
        roomId = formData.get("room-id");
        if(roomId){
            console.log("Оновлення кімнати");
            fetch('/api/room',{
                method:'PUT',
                body:formData
            }).then(r=>
            {
                if(r.status<300){
                    window.location.reload();
                }
                else{
                    r.text().then(alert);
                }
            });
        }
        else{
            console.log("Додавання кімнати");
            fetch("/api/room",{
                method:'POST',
                body:formData
            }).then(r=>
            {console.log(r);
                if(r.status==201){

                    window.location.reload();
                }
                else{
                    r.text().then(alert);
                }
            });
        }
       
        
    }
    if(form.id == 'category-form')
    {
        e.preventDefault();
        // перевіряємо чи це додавання чі редагування
        // Ознака - наявності поля category-id
        let formData = new FormData(form);
        ctgId = formData.get("category-id");
        if(ctgId){
            // оновлення
            console.log("Оновлення категорії"+ctgId);
            fetch('/api/category',{
                method:'PUT',
                body:formData
            }).then(r=>
            {
                if(r.status<300){
                    window.location.reload();
                }
                else{
                    r.text().then(alert);
                }
            });
        }
        else {
            // додавання
            console.log("Додавання новоі категорії"+ctgId);
            fetch('/api/category',{
                method:'POST',
                body:formData
            }).then(r=>
            {
                if(r.status==201){
                   window.location.reload();
                }
                else{
                    r.text().then(alert);
                }
            });
        }
    }
    if(form.id=='location-form')
    {
        e.preventDefault();
        let formData = new FormData(form);
        locId = formData.get('location-id');
        if(!locId){
            console.log("Додавання нової локаціі");
            fetch('/api/location', {
                method: 'POST',
                body: formData
            }).then(r=>{
                console.log(r);
                if(r.status<300){
                    window.location.reload();
                }
                else{
                    r.text().then(alert);
                }
            })
        }
        else {
            console.log("Оновлення локації");
            fetch('/api/location', {
                method: 'PUT',
                body: formData
            }).then(r=>{
                console.log(r);
                if(r.status<300){
                    window.location.reload();
                }
                else{
                    r.text().then(alert);
                }
            })
        }
        
        
        
    }
    // на інші форми ми не вплюваємо
});

document.addEventListener('DOMContentLoaded',function (){
   const autorButton = document.getElementById("auth-button");
   if(autorButton){
       autorButton.addEventListener('click',authButtonClick);
   }
    const confirmEmail = document.getElementById("confirm-email-button");
    if(confirmEmail){
        confirmEmail.addEventListener('click',confirmEmailClick);
    }
    serveReserveButtons();
    serveAdminButtons();
    initAdminPage();
   
});
function confirmEmailClick(){
    const emailCodeInput = document.getElementById("email-code");
    if(!emailCodeInput){
        throw "Element email-code not found!";
    }
    const emailMessage = document.getElementById("email-message");
    if(!emailMessage){
        throw "Element email-message not found!";
    }
    const code = emailCodeInput.value.trim();
    if(!code){
        emailMessage.classList.remove('visually-hidden');
        emailMessage.innerText="Необхідно ввести код!";

        return;
    }
    const email = emailCodeInput.getAttribute('data-email');
    fetch(`/api/auth?email=${email}&code=${code}`,{method:'PATCH'}).then(r=>{
        if(r.status==202){
           window.location.reload();
        }
        else{
            emailMessage.classList.remove('visually-hidden');
            emailMessage.innerText="Код не прийнято!";
        }
    })
}
function serveAdminButtons(){
    for(let btn of document.querySelectorAll('[data-type="edit-location"]')){
        ///location
        btn.addEventListener('click', e => {
            let b = e.target.closest('[data-type="edit-location"]');
            document.querySelector('[name="location-id"]').value =
                b.getAttribute("data-location-id");
            document.querySelector('[name="location-name"]').value =
                b.getAttribute("data-location-name");
            document.querySelector('[name="location-description"]').value =
                b.getAttribute("data-location-description");
            document.querySelector('[name="location-slug"]').value =
                b.getAttribute("data-location-slug");
            ///
            document.querySelector('[name="location-stars"]').value =
                b.getAttribute("data-location-stars");
        });
    }
    for(let btn of document.querySelectorAll('[data-type="edit-room"]')){
        ///room
        btn.addEventListener('click', e => {
            let b = e.target.closest('[data-type="edit-room"]');
            document.querySelector('[name="room-id"]').value =
                b.getAttribute("data-room-id");
            document.querySelector('[name="room-name"]').value =
                b.getAttribute("data-room-name");
            document.querySelector('[name="room-description"]').value =
                b.getAttribute("data-room-description");
            document.querySelector('[name="room-slug"]').value =
                b.getAttribute("data-room-slug");
            ///
            document.querySelector('[name="room-stars"]').value =
                b.getAttribute("data-room-stars");
            document.querySelector('[name="room-price"]').value =
                b.getAttribute("data-room-price");
        });
    }
    for(let btn of document.querySelectorAll('[data-type="edit-category"]')){
        btn.addEventListener('click', e => {
            let b = e.target.closest('[data-type="edit-category"]');
            document.querySelector('[name="category-id"]').value =
                b.getAttribute("data-category-id");
            document.querySelector('[name="category-name"]').value =
                b.getAttribute("data-category-name");
            document.querySelector('[name="category-description"]').value =
                b.getAttribute("data-category-description");
            document.querySelector('[name="category-slug"]').value =
                b.getAttribute("data-category-slug");
        });
    }
    for (let btnDelete of document.querySelectorAll('[date-type="delete-category"]')){
        btnDelete.addEventListener('click', e => {
            let b = e.target.closest('[date-type="delete-category"]');
            let id = b.getAttribute("data-category-id");
            if(id){
                if(confirm("Вы действительно хотите удалить категорию ?")){
                    fetch(`/api/category/${id}`,{method: 'DELETE'}).then(r=>{
                        if(r.status < 400){
                            window.location.reload();
                        }
                        else{
                            alert("Ошибка при удалении!");
                        }
                    })
                }
            }
            else{
                alert("Ошибка разметки - нет id элемента!");
            }
        });
    }
    for (let btnDelete of document.querySelectorAll('[date-type="delete-location"]')){
        btnDelete.addEventListener('click', e => {
            let b = e.target.closest('[date-type="delete-location"]');
            let id = b.getAttribute("data-location-id");
            if(id){
                if(confirm("Вы действительно хотите удалить локацию ?")){
                    fetch(`/api/location/${id}`,{method: 'DELETE'}).then(r=>{
                        if(r.status < 400){
                            window.location.reload();
                        }
                        else{
                            alert("Ошибка при удалении!");
                        }
                    })
                }
            }
            else{
                alert("Ошибка разметки - нет id элемента!");
            }
        });
    }
    ////room
    for (let btnDelete of document.querySelectorAll('[date-type="delete-room"]')){
        btnDelete.addEventListener('click', e => {
            let b = e.target.closest('[date-type="delete-room"]');
            let id = b.getAttribute("data-room-id");
            if(id){
                if(confirm("Вы действительно хотите удалить комнату ?")){
                    fetch(`/api/room/${id}`,{method: 'DELETE'}).then(r=>{
                        if(r.status < 400){
                            window.location.reload();
                        }
                        else{
                            alert("Ошибка при удалении!");
                        }
                    })
                }
            }
            else{
                alert("Ошибка разметки - нет id элемента!");
            }
        });
    }
    for (let btnRest of document.querySelectorAll('[date-type="restore-category"]')) {
        btnRest.addEventListener('click', e => {
            let b = e.target.closest('[date-type="restore-category"]');
            let id = b.getAttribute("data-category-id");
            if (id) {
                if (confirm("Вы действительно хотите востановить категорию ?")) {
                    fetch(`/api/category?id=${id}`, {method: 'RESTORE'}).then(r => {
                        if (r.status < 400) {
                            window.location.reload();
                            //r.text().then(console.log);
                        } else {
                            alert("Ошибка при востановлении!");
                        }
                    })
                }
            } else {
                alert("Ошибка разметки - нет id элемента!");
            }
        });
    }
    /////// loc
    for (let btnRest of document.querySelectorAll('[date-type="restore-location"]')) {
        btnRest.addEventListener('click', e => {
            let b = e.target.closest('[date-type="restore-location"]');
            let id = b.getAttribute("data-location-id");
            if (id) {
                if (confirm("Вы действительно хотите востановить локацию ?")) {
                    fetch(`/api/location?id=${id}`, {method: 'RESTORE'}).then(r => {
                        if (r.status < 400) {
                            window.location.reload();
                            //r.text().then(console.log);
                        } else {
                            alert("Ошибка при востановлении!");
                        }
                    })
                }
            } else {
                alert("Ошибка разметки - нет id элемента!");
            }
        });
    }
    ////// room
    for (let btnRest of document.querySelectorAll('[date-type="restore-room"]')) {
        btnRest.addEventListener('click', e => {
            let b = e.target.closest('[date-type="restore-room"]');
            let id = b.getAttribute("data-room-id");
            if (id) {
                if (confirm("Вы действительно хотите востановить комнату ?")) {
                    fetch(`/api/room?id=${id}`, {method: 'RESTORE'}).then(r => {
                        if (r.status < 400) {
                            window.location.reload();
                            //r.text().then(console.log);
                        } else {
                            alert("Ошибка при востановлении!");
                        }
                    })
                }
            } else {
                alert("Ошибка разметки - нет id элемента!");
            }
        });
    }
}
function addNewRoom(){
    const roomNameInput = document.getElementById('room-name');
    const alertNameRoom = document.getElementById('room-name-alert');
    
    const roomDescriptionInput = document.getElementById('room-description');
    const AlertRoomDescription = document.getElementById('room-description-alert');
    
    const slugRoom = document.getElementById('room-slug');
    const slugRoomAlert = document.getElementById('room-slug-alert');
    
    const starsRoom = document.getElementById('room-stars')
    const starsRoomAlert = document.getElementById('room-stars-alert')
    
    const priceRoom = document.getElementById('room-price');
    const priceRoomAlert = document.getElementById('room-price-alert');
    
    const roomPhoto =document.getElementById('room-photo');
    const roomPhotoAlert = document.getElementById('room-photo-alert');
    if (!roomNameInput) {
        throw "Element room-name not found!";
    }
    if (!alertNameRoom) {
        throw "Element room-name-alert not found!";
    }
    if(!roomDescriptionInput){
        throw "Element room-description not found!"
    }
    if(!AlertRoomDescription){
        throw "Element room-description-alert not found!"
    }
    if (!slugRoom){
        throw "Element room-slug not found!"
    }
    if(!slugRoomAlert){ 
        throw "Element room-slug-alert not found!"
    }
    if(!starsRoom){
        throw "Element room-stars not found!"
    }
    if(!starsRoomAlert){
        throw "Element room-stars-alert not found!"
    }
    if(!priceRoom){
        throw "Element room-price not found!"
    }
    if(!priceRoomAlert){
        throw "Element room-price-alert not found!"
    }
    if(!roomPhoto){
        throw "Element room-photo not found!"
    }
    if(!roomPhotoAlert){
        throw "Element room-price-alert not found!"
    }
    const nameValue = roomNameInput.value.trim();
    if(nameValue === ""){
        alertNameRoom.classList.remove('visually-hidden');
        alertNameRoom.innerText = "Название обязательное!";
        return false;
    }
    const descriptionDescriptionValue = roomDescriptionInput.value.trim();
    if(descriptionDescriptionValue===""){
        AlertRoomDescription.classList.remove("visually-hidden");
        AlertRoomDescription.innerText="Описание комнаты обязательное!";
        return false;
    }
    const slugRoomValue = slugRoom.value.trim();
    if(slugRoomValue===""){
        slugRoomAlert.classList.remove('visually-hidden');
        slugRoomAlert.innerText = "Slug дожден быть заполнен, для понятной адресации.";
        return false;
    }
    const starsRoomValue = starsRoom.value.trim();
    if(starsRoomValue===""){
        starsRoomAlert.classList.remove('visually-hidden');
        starsRoomAlert.innerText = "Поле Stars должно быть заполнено, для оценки номера!";
        return false;
    }
    const priceRoomValue = priceRoom.value.trim();
    if(priceRoomValue===""){
        priceRoomAlert.classList.remove('visually-hidden');
        priceRoomAlert.innerText = "Поле Price должно быть заполнено.";
        return false;
    }
    if (roomPhoto.files.length === 0) {
        roomPhotoAlert.classList.remove('visually-hidden');
        roomPhotoAlert.innerText = "Фотография комнаты должна быть загружена!";
    }
    return true;
}

function authButtonClick() {
    const authEmail = document.getElementById("auth-name");
    if(!authEmail){
        throw "Element auth-name not found!";
    }
    const alertErrorEmail = document.getElementById("auth-emailError");
    if(!alertErrorEmail){
        throw "Element auth-emailError not found!";
    }
    const alertErrorPassword = document.getElementById("auth-passwordError");
    if(!alertErrorPassword) {
        throw "Element auth-passwordError not found!";
    }
    const authPassword = document.getElementById("auth-password");
    if(!authPassword){
        throw "Element auth-password not found!";
    }
    const authMessage = document.getElementById("auth-message");
    if(!authMessage){
        throw "Element auth-message not found!";
    }
    const email = authEmail.value.trim();
    const password = authPassword.value.trim();
    if(!email){
        alertErrorEmail.classList.remove('visually-hidden');
        alertErrorEmail.innerText="Необхідно ввести Email!";
        
        return;
    }
    if(!password){
        alertErrorPassword.classList.remove('visually-hidden');
        alertErrorPassword.innerText="Необхідно ввести Password!";
        return;
    }
    else{
        alertErrorEmail.classList.add('visually-hidden');
        alertErrorPassword.classList.add('visually-hidden');
        
    }
    fetch(`/api/auth?email=${email}&password=${password}`).then(r=>{
        if(r.status!=200)
        {
            authMessage.classList.remove('visually-hidden');
            authMessage.innerText=" Вхід скасовано, перевірте введені данні!!";
        }
        else
        {
            window.location.reload();
        }
        console.log(r)});
}

////////// ADMIN-PAge
function initAdminPage(){
    loadCategories();
}
function loadCategories(){
    const container =document.getElementById("category-container");
    if(!container){
        throw "Element category-container not found!"
    }
    const inputcategory = document.getElementById("inputs-category-container");
    if(!inputcategory){
        throw "Element inputs-category-container not found!";
    }
    fetch("/api/category") // запитуємо наявні категоріі
        .then(r=>r.json())
        .then(j=>{
            let html ="";
            let inputHtml = "";
            for (let ctg of j) {
                html += `<p data-id="${ctg["id"]}" onclick="ctgClick('${ctg["id"]}')">${ctg["name"]}</p>`;
            }
            inputHtml += `Назва: <input id="ctg-name" /><br/>
            Опис: <textarea id="ctg-description"></textarea><br/>
            Фото: <input type="file" id="ctg-photo"></input><br/>
            <button onclick='addCategory()'>Add</button>`;
            container.innerHTML = html;
            inputcategory.innerHTML = inputHtml;
        })
}
function ctgClick(ctgId) {
    fetch("/api/location/" + ctgId)
        .then(r => r.json())
        .then(j => {
            const container = document.getElementById("location-container");
            let html = "";
            for (let loc of j) {
                html += `<p data-id="${loc["id"]}" onclick="locClick(event)">${loc["name"]}</p>`;
            }
            html += `Назва: <input id="loc-name" /><br/>
            Опис: <textarea  id="loc-description"></textarea><br/>
            Рейтинг: <input id="loc-stars" type="number" /><br/>
            Фото: <input id="loc-photo" type="file" /><br/>
            <button onclick='addLocation("${ctgId}")'>+</button>`;
            container.innerHTML = html;
        });
}
function loadLocation(){
    const container = document.getElementById()
}
function addCategory(){
   const ctgName = document.getElementById("ctg-name").value;
   const ctgDescription = document.getElementById("ctg-description").value;
    const ctgPhoto = document.getElementById("ctg-photo")
   if( confirm(`Додаємо категорію ${ctgName} ${ctgDescription} ?`)){
       let formData = new FormData();
       formData.append("name", ctgName);
       formData.append("description", ctgDescription);
       formData.append("photo", ctgPhoto.files[0]);
       fetch("/api/category",{method:'POST',
           body:formData
       }).then(r=>{
           if(r.status==201){
               loadCategories();
           }
           else {
               alert('Error!');
           }
           console.log(r)
       });
           
      // alert('ok');
   }
  
}

function addLocation(ctgId){
    const ctgName = document.getElementById("loc-name").value;
    const ctgDescription = document.getElementById("loc-description").value;
    const ctgStars = document.getElementById("loc-stars").value;
    const locPhoto = document.getElementById("loc-photo")
    if( confirm(`Додаємо локацію ${ctgName} ${ctgDescription} ${ctgStars} ?`)){
        let formData = new FormData();
        formData.append("categoryId", ctgId);
        formData.append("name", ctgName);
        formData.append("description", ctgDescription);
        formData.append("stars", ctgStars);
        formData.append("photo", locPhoto.files[0]);
        fetch("/api/location", {
            method: 'POST',
            body: formData
        }).then(r=>{
            if(r.status==201){
                ctgClick(ctgId);
            }
            else {
                alert('Error!');
            }
            console.log(r)
        });

        // alert('ok');
    }
}

function serveReserveButtons() {
    for (let btn of document.querySelectorAll('[data-type="reserve-room"]')) {
        btn.addEventListener('click', e => {
            const cont = e.target.closest('[data-type="reserve-room"]');
            const roomId = cont.getAttribute('data-room-id');
            const userId = cont.getAttribute('data-user-id');
            const name = cont.getAttribute('data-room-name');
            const date = cont.getAttribute('data-date');

            console.log(roomId, userId, date);
            if(confirm(`Вы уверены, что хотите забронировать номер: ${name} на дату: ${date} ?`))
            {
                fetch('/api/room/reserve', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({
                        date,
                        roomId,
                        userId
                    })
                }).then(r => {
                    if (r.status === 201) {
                        window.location.reload();
                    }
                    else {
                        r.text().then(alert);
                    }
                });
            }
            
        });
    }

    for (let btn of document.querySelectorAll('[data-type="drop-reserve-room"]')) {
        btn.addEventListener('click', e => {
            const cont = e.target.closest('[data-type="drop-reserve-room"]');
            const reservId = cont.getAttribute('data-reserve-id');
            const roomName = cont.getAttribute('data-room-name-cancellation');
            const roomDate = cont.getAttribute('data-room-reserve-date');
            const trimmedDate = roomDate.substring(0,10);
            //console.log(roomId, userId, date);
            if(!confirm(`Чи підтверджуєте ви скасування заброньованого номера: ${roomName}, на дату: ${trimmedDate}`))
            {
                return;
            }
            fetch('/api/room/reserve?reservId='+reservId, {
                method: 'DELETE',
            }).then(r => {
                if (r.status === 202) {
                    window.location.reload();
                }
                else {
                    r.text().then(alert);
                }
            });

        });
    }
}