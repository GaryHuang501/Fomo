import MockSnapshot from "./MockSnapshot";

export default class MockFireBaseRef {
    constructor(path) { 
        this.path = path;

        if(MockFireBaseRef.messageQueue == null){
            MockFireBaseRef.messageQueue = [];
        }
    }

    on(event, callback) {
        this.onEvent = event;
        this.callback = callback;
    }

    off(event){
        this.offEvent = event;
    }

    set(value){
        MockFireBaseRef.messageQueue.push(value);
    }

    push(){
        this.key = Math.random().toString();
        return this;
    }

    limitToLast(){      
        return this;
    }

    invokeCallBack(snapshot){
        this.callback(snapshot);
    }

    invokeCallAllPending(){
        let message = MockFireBaseRef.messageQueue.shift();

        while(message){      
            this.invokeCallBack(new MockSnapshot(message));
            message = MockFireBaseRef.messageQueue.shift();
        }
    }
    
    reset(){
        MockFireBaseRef.messageQueue = [];
    }
}