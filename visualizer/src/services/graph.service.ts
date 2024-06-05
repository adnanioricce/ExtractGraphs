const settings = {
    API_URL: "http://localhost:5000"
}
export type GraphData = {
    [key: string]: string[];
};
/**
 * 
 * @param url: string of a url web page to generate the graph
 */
export async function generateGraphFromPage(url:string) {
    const apiUrl = settings.API_URL
    const targetUrl = `${apiUrl}/graphs/${url}`
    const response = await fetch(targetUrl)
    const data = await response.json()
    
}
export async function fetchGraphData(): Promise<GraphData> {
    const response = await fetch('http://localhost:5173/stripLinks.out.json');
    const data: GraphData = await response.json();        
    return data;
};
