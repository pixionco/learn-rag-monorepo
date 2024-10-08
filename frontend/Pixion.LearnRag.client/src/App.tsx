import "@xyflow/react/dist/style.css";
import { QueryClient, QueryClientProvider } from "react-query";

const queryClient = new QueryClient();

function App() {
  return (
    <QueryClientProvider client={queryClient}>
      <span>temp</span>
    </QueryClientProvider>
  );
}

export default App;
