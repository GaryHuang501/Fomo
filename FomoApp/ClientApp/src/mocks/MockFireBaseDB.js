import MockFirebaseRef from './MockFirebaseRef';

export default function MockFirebaseDB(){

    if(MockFirebaseDB.refs == null){
        MockFirebaseDB.refs = [];
    }

    this.ref = function(path) {
        const ref = new MockFirebaseRef(path);
        MockFirebaseDB.refs.push(ref);
        return ref;
    }

    this.refs = MockFirebaseDB.refs;

    this.reset = function(){
        for(const ref of MockFirebaseDB.refs){
            ref.reset();
        }
        
        MockFirebaseDB.refs = [];
    }

    return {
        ref: this.ref, 
        refs: this.refs,
        reset: this.reset
    };
};
