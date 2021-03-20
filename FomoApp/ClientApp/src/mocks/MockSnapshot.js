export default class MockSnapshot{

    constructor(value){
        this.value = value;
    }

    val(){
        return this.value;
    }

    test(){
        return 'yo';
    }
}