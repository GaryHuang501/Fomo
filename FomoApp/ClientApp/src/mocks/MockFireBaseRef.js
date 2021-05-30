import MockSnapshot from "./MockSnapshot";

export default class MockFireBaseRef {
    constructor(path) { 
        this.path = path;

        if(this.messageQueue == null){
            this.messageQueue = [];
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
        this.messageQueue.push(value);
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
        let message = this.messageQueue.shift();

        while(message){      
            this.invokeCallBack(new MockSnapshot(message));
            message = this.messageQueue.shift();
        }
    }
    
    reset(){
        this.messageQueue = [];
    }
}