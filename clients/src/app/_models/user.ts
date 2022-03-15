export interface User {
    username: string;
    token: string;
}

let data:number | string = 42;

data = "10";
interface Car{
    color:string;
    model:string;
    topSpeed?:number;
}
const car1:Car=
{
    color:'blue',
    model:'BMW',
}

const car2=
{
    color:'blue',
    model:'BMW',
    topSpeed:100
}

const multyply = (x,y)=>{
    return x*y;
}