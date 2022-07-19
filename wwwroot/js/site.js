const hubConnection = new signalR.HubConnectionBuilder()
    .withUrl("/chat")
    .withAutomaticReconnect()
    .build();

let block = document.getElementsByClassName("print-message")[0];
block.scrollTop = block.scrollHeight;

function getFormatDate(date){
    let now = new Date(date);

    let year = now.getFullYear();
    let month = now.getMonth();
    let day = now.getDate();

    let hour = now.getHours();
    let minute = now.getMinutes().toString().padStart(2, '0');

    return `${day}.${month}.${year} ${hour}:${minute}`;
}




function addItemMessage(itemMessage) {

    let print = document.getElementById("print");

    let message = document.createElement('div');
    message.classList.add('message');
    message.classList.add("right");

    let info = document.createElement('div');
    info.classList.add('info');

    let username = document.createElement('span');
    username.classList.add('username');

    let publishedDate = document.createElement('span');
    publishedDate.classList.add('published-date');


    let textMessage = document.createElement('span');
    textMessage.classList.add('text');

    username.innerText = itemMessage.userName;

    publishedDate.innerText = getFormatDate(itemMessage.publishedDate);

    textMessage.innerText = itemMessage.text;

    // for(var symb of itemMessage.text)
    // {
    //     console.log(`${symb} ${symb.charCodeAt(0)}`);
    // }

    info.appendChild(username);
    info.appendChild(publishedDate);

    message.appendChild(textMessage);
    message.appendChild(info);
    print.appendChild(message);

    block.scrollTop = block.scrollHeight;
}

function setStatus(status, userName){
    let linkLogin = document.getElementById(`login-${userName}`);

    let statusLogin = linkLogin.childNodes[7];

    let newStatusLogin = document.createElement("span");

    newStatusLogin.classList.add('username');
    newStatusLogin.classList.add('status');
    newStatusLogin.classList.add(status);
    newStatusLogin.innerText = status;

    linkLogin.replaceChild(newStatusLogin, statusLogin);
}

hubConnection.on("Receive", addItemMessage);

hubConnection.on("SetStatus", setStatus);

function invokeSend() {
    let text = document.getElementById("Text");

    hubConnection.invoke("Send", window.location.pathname, text.innerText);

    text.innerText = "";
}

let sender = document.getElementById("send");

if (sender){
    sender.addEventListener('click', invokeSend);

    sender.addEventListener('keypress', function (event) {
        if (event.key === "Enter") {
            event.preventDefault();
            invokeSend();
        }
    });
}

hubConnection.start();