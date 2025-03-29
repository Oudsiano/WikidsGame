mergeInto(LibraryManager.library, {
  ConnectToSocket: function () {
    if (!window.socket) {
      window.socket = io("http://localhost:3000");

      function safeSendMessage(obj, method, param) {
        try {
          if (typeof SendMessage === "function") {
            if (param !== undefined) {
              SendMessage(obj, method, param);
            } else {
              SendMessage(obj, method);
            }
          } else {
            setTimeout(() => safeSendMessage(obj, method, param), 100);
          }
        } catch (e) {
          console.error(`SendMessage ${method} failed:`, e);
          setTimeout(() => safeSendMessage(obj, method, param), 100);
        }
      }

      window.socket.on("connect", function () {
        console.log("Connected to server!");
        safeSendMessage("SocketManager", "OnConnected");
      });

      window.socket.on("playerConnected", function (data) {
        safeSendMessage("SocketManager", "OnPlayerConnected", data.id);
      });

      window.socket.on("playerData", function (data) {
        const json = JSON.stringify(data);
        safeSendMessage("SocketManager", "OnPlayerData", json);
      });

      window.socket.on("youAre", function (data) {
        safeSendMessage("SocketManager", "OnYouAre", data.id);
      });

      window.socket.on("existingPlayers", function (playerIds) {
        const json = JSON.stringify({ playerIds: playerIds });
        console.log("👉 existingPlayers json:", json);
        safeSendMessage("SocketManager", "OnExistingPlayers", json);
      });

      window.socket.on("playerDisconnected", function (data) {
        safeSendMessage("SocketManager", "OnPlayerDisconnected", data.id);
      });

      window.socket.on("serverFull", function (data) {
        console.warn(data.message);
        safeSendMessage("SocketManager", "OnServerFull", data.message);
      });
    }
  },

  SendPlayerData: function (data) {
    if (window.socket && data) {
      try {
        var str = UTF8ToString(data);
        window.socket.emit("playerData", str);
      } catch (e) {
        console.error("SendPlayerData failed:", e);
      }
    }
  },

  DisconnectSocket: function () {
    if (window.socket) {
      try {
        window.socket.disconnect();
      } catch (e) {
        console.error("DisconnectSocket failed:", e);
      }
    }
  }
});
