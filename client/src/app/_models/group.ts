export interface Group {
    username: string;
    connections:Connection[];
}

interface Connection {
    connnectionId: string;
    username: string;

}