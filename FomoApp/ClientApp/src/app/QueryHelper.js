export default {
    createIdsQuery: function(key, ids){
        
        let idQuery = "";
        
        if(!key){
            return idQuery;
        }

        for(let i = 0; i < ids.length; i++){
            const id = ids[i];

            if(i !== 0){
                idQuery += "&"
            }

            idQuery += `${key}=${id}`
        }

        return idQuery;
    }
}