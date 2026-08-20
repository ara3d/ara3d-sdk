// Wave-9 Track E fence: effectful export sinks. Evaluate is PURE (design §6):
// it reports readiness and passes the table through; the integration layer's
// Run button performs the actual POST /api/export-csv. The filename's schema
// default ("export.csv") is applied by the evaluator's withDefaults, so an
// empty filename only happens on a direct call — and reads as needs-setup,
// not an error.
import { strParam, tableIn } from "./lib";
import { def } from "./registry";
import { needsSetup } from "./types";

def("sink.exportCsv", async (n, inputs) => {
  const table = tableIn(inputs, "in");
  const filename = strParam(n, "filename");
  if (!filename) needsSetup("choose a filename");
  return { value: table, summary: `ready: ${table.rows.length} rows → ${filename}` };
});
