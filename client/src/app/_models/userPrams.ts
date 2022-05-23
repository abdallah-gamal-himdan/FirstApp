import { User } from "./user";

export class Userprams{
    gender : string;
    minAge = 18 ;
     maxAge = 99;
     pageNumber = 1;
     pageSize = 5;
     orderBy="lastActive";


     constructor (user : User){
         this.gender = user.Gender ==="female"?'male' : 'female';
     }
}