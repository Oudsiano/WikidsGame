mergeInto(LibraryManager.library, {
    /**
     * Инициализирует соединение с Unity-колбэками
     */
    InitializeSocketWithCallbacks: function () {
        if (!window.clientConnector) {
            console.error("window.clientConnector not found");
            return;
        }

        if (!window.unityCallbacks) {
            console.error("Unity callbacks structure is not initialized");
            return;
        }

        // Инициализируем clientConnector, передавая готовую структуру колбэков
        window.clientConnector.initialize({ modules: window.unityCallbacks });
    },

    /**
     * Регистрирует Unity-колбэк для компонента
     */
    RegisterUnityCallback: function (componentNamePtr, methodNamePtr, callbackFuncPtr) {
        const componentName = UTF8ToString(componentNamePtr);
        const methodName = UTF8ToString(methodNamePtr);

        // Убедимся, что структура unityCallbacks существует
        if (!window.unityCallbacks) {
            window.unityCallbacks = {};
        }

        if (!window.unityCallbacks[componentName]) {
            window.unityCallbacks[componentName] = {};
        }

        // Регистрируем функцию обратного вызова
        window.unityCallbacks[componentName][methodName] = function (jsonData) {
            const bufferSize = lengthBytesUTF8(jsonData) + 1;
            const buffer = _malloc(bufferSize);
            stringToUTF8(jsonData, buffer, bufferSize);
            dynCall_vi(callbackFuncPtr, buffer);
            _free(buffer);
        };

        console.log(`Registered Unity callback: ${componentName}.${methodName}`);
    },

    /**
     * Вызывает методы компонентов через unity прослойку
     */
    InvokeConnectorMethod: function (componentNamePtr, methodNamePtr, jsonDataPtr) {
        if (!window.clientConnector || !window.clientConnector.unity) {
            console.error("window.clientConnector not found");
            return;
        }

        const componentName = UTF8ToString(componentNamePtr);
        const methodName = UTF8ToString(methodNamePtr);
        const jsonData = UTF8ToString(jsonDataPtr);

        const component = window.clientConnector.unity[componentName];
        if (!component || !component[methodName]) {
            console.error(`Unity method ${componentName}.${methodName} not found`);
            return;
        }

        component[methodName](jsonData);
    },

    /**
     * Вызывает базовые методы самого объекта clientConnector 
     */
    CallCoreConnectorMethod: function (methodNamePtr) {
        try {
            const methodName = UTF8ToString(methodNamePtr);
            const method = window.clientConnector[methodName];
            
            if (typeof method !== 'function') {
                return JSON.stringify({ error: `Method ${methodName} not found` });
            }
            
            const result = method();
            return typeof result === 'object' ? 
                JSON.stringify(result) : 
                JSON.stringify({ result: result });
        } catch (error) {
            return JSON.stringify({ error: error.message || "Unknown error" });
        }
    },

    CallNamespacedMethod: function (namespacePtr, methodNamePtr) {
    try {
        const ns = UTF8ToString(namespacePtr);
        const methodName = UTF8ToString(methodNamePtr);
        const target = window.clientConnector[ns];

        if (!target || typeof target[methodName] !== 'function') {
            return JSON.stringify({ error: `Method ${ns}.${methodName} not found` });
        }

        const result = target[methodName]();
        return typeof result === 'object'
            ? JSON.stringify(result)
            : JSON.stringify({ result: result });
    } catch (error) {
        return JSON.stringify({ error: error.message || "Unknown error" });
        }
    },

    CallNamespacedMethodWithArg: function (namespacePtr, methodNamePtr, jsonArgPtr) {
    try {
        const ns = UTF8ToString(namespacePtr);
        const methodName = UTF8ToString(methodNamePtr);
        const jsonArg = UTF8ToString(jsonArgPtr);

        const arg = JSON.parse(jsonArg);
        const target = window.clientConnector[ns];

        if (!target || typeof target[methodName] !== 'function') {
            return JSON.stringify({ error: `Method ${ns}.${methodName} not found` });
        }

        const result = target[methodName](arg);
        return typeof result === 'object'
            ? JSON.stringify(result)
            : JSON.stringify({ result: result });
        } catch (error) {
            return JSON.stringify({ error: error.message || "Unknown error" });
        }
    },

    SendWebGLMessage: function (componentNamePtr, methodNamePtr, jsonDataPtr) {
    if (!window.clientConnector || !window.clientConnector.unity) {
        console.error("clientConnector.unity not available");
        return;
    }
    
    const componentName = UTF8ToString(componentNamePtr);
    const methodName = UTF8ToString(methodNamePtr);
    const jsonData = UTF8ToString(jsonDataPtr);
    
    if (!window.clientConnector.unity[componentName]) {
        console.error(`Unity component ${componentName} not found`);
        return;
    }
    
    if (!window.clientConnector.unity[componentName][methodName]) {
        console.error(`Unity method ${componentName}.${methodName} not found`);
        return;
    }
    
    // Вызываем метод из прокси для Unity
    // Прокси автоматически преобразует JSON-строку в объект
    window.clientConnector.unity[componentName][methodName](jsonData);
    },

    DestroySocket: function () {
    if (window.clientConnector && window.clientConnector.destroy) {
        window.clientConnector.destroy();
    }
    
    if (window.unityCallbacks) {
        window.unityCallbacks = null;
        }
    },

        IsSocketInitialized: function () {
        if (!window.clientConnector) {
            return 0;
        }
        return window.clientConnector.isInitialized() ? 1 : 0;
    },

    ConfirmRoom: function () {
    if (!window.clientConnector || !window.clientConnector.room) {
        console.error("clientConnector.room not available");
        return;
    }
    window.clientConnector.room.confirm();
    console.log("[AUTO] Sending room.confirm()");
    },

    FastJoinToRoom: function () {
    console.log("FastJoinToRoom");
     if (!window.clientConnector) {
        console.error(" clientConnector is NOT defined at TestJSLib call time.");
    } else {
        window.clientConnector.room.fastJoin();
         console.log("FastJoinToRoom is succesful!");
    }
    },

    GetUserProfile: function () {
    console.log("GetUserProfile");
     if (!window.clientConnector) {
        console.error(" clientConnector is NOT defined at TestJSLib call time.");
    } else {
        window.clientConnector.user.getProfile();
         console.log("GetProfile is succesful!");
    }
    },

    TestJSLib: function () {
        console.log("JSLib works!");
    },

    GetAllPlayersInScene: function(playerId) {
        console.log("GetAllPlayersInScene");
        window.clientConnector.game.mirrorAction({
            method: "getAllPlayersInScene",
            playerId: playerId,
            params: {}
        });
    },

    SendHealth: function(playerId, health) {
        window.clientConnector.game.mirrorAction({
            method: "sendHealth",
            playerId: playerId,
            params: {
                health: health
            }
        });
    }
});
