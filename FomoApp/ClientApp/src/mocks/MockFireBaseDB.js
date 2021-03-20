import MockFireBaseRef from './MockFireBaseRef';

export default function MockFireBaseDB(){

    if(MockFireBaseDB.refs == null){
        MockFireBaseDB.refs = [];
    }

    this.ref = function(path) {
        const ref = new MockFireBaseRef(path);
        MockFireBaseDB.refs.push(ref);
        return ref;
    }

    this.refs = MockFireBaseDB.refs;

    this.reset = function(){
        for(const ref of MockFireBaseDB.refs){
            ref.reset();
        }
        
        MockFireBaseDB.refs = [];
    }

    return {
        ref: this.ref, 
        refs: this.refs,
        reset: this.reset
    };
};
