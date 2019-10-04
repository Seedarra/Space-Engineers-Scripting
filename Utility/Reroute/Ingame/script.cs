/*
 * See Darra - 10/2019
 * Space Engineers - Reroute
 * 1.0.0
 * 
 * This serves as a demonstration on how to use Reroute.
 */

public class GoodClass
{
	public GoodClass(Program parent)
	{
		parent.Echo("Reroute says hi!");
		parent.Echo("I support linebreaks with \"\\n\" but i also try to split up lines automatically that are to long to fit the screen.");
	}
}

public class BadClass
{
	public BadClass(Program parent)
	{
		parent.Echo("Beware the bad class!");
		throw new Exception("BadClass class has failed (according to plan).");
	}
}

public Program()
{
	Reroute console = new Reroute(this);

	try {
		console.Compile("Good Class");

		GoodClass gc = new GoodClass(this);

		console.Compile("Bad Class");

		BadClass bc = new BadClass(this);

		console.Report();
	}
	catch(Exception e) {
		console.Cancel(e.Message);
	}
}

public void Main()
{

}

public class Reroute
{
	public Reroute(Program parent)
	{
		gParent = parent;
		gEcho = gParent.Echo;
		gScreen = gParent.Me.GetSurface(0);

		lBuffer = new List<string>();

		// Set up display:
		gScreen.Font = "DEBUG";
		gScreen.FontSize = gParent.Me.CubeGrid.GridSize == 2.5 ? 0.62f : 1.0f;
		gScreen.FontColor = new Color(179, 237, 255);
		gScreen.BackgroundColor = new Color(20, 20, 20);
		gScreen.TextPadding = 2.0f;
		gScreen.Alignment = TextAlignment.LEFT;
		gScreen.ContentType = ContentType.TEXT_AND_IMAGE;

		if(gScreen.GetText() != string.Empty && gScreen.GetText()[0] != '#')
			gScreen.WriteText("", false);

		// Determine screen size limits:
		Vector2 screensize = gScreen.SurfaceSize;

		if(gParent.Me.CubeGrid.GridSize != 2.5)
			screensize = new Vector2(472.0f, 512.0f);

		StringBuilder sb = new StringBuilder("|");

		float padding = 1.0f - (2.0f * gScreen.TextPadding / 100.0f);
		float height = gScreen.MeasureStringInPixels(sb, gScreen.Font, gScreen.FontSize).Y;

		vTextLimit = new Vector2(padding * screensize.X, (padding * screensize.Y) / height);
	}

	public void Compile(string user)
	{
		if(sUser != string.Empty)
			Report();

		sUser = user != string.Empty ? user : "<UNKNOWN>";

		lBuffer.Clear();
		lBuffer.Add($"# ./compile {(sUser.Contains(' ') ? $"\"{sUser}\"" : sUser)}");

		gParent.Echo = Echo;
	}

	public void Report()
	{
		if(sUser == string.Empty)
			return;

		lBuffer.Add($"# {sUser} ready.");

		gParent.Echo = gEcho;
		gParent.Echo($"{sUser} ready.");

		sUser = "";

		WriteBuffer();
	}

	public void Cancel(string message)
	{
		if(sUser != string.Empty) {
			sUser = "";

			gParent.Echo = gEcho;

			AppendToBuffer($"# !!! {message}");

			WriteBuffer();
		}
		gParent.Echo(message);
	}

	private readonly Program gParent;
	private readonly Action<string> gEcho;
	private readonly IMyTextSurface gScreen;
	private readonly Vector2 vTextLimit;
	private readonly List<string> lBuffer;

	private string sUser = "";

	private void Echo(string message)
	{
		if(sUser != string.Empty)
			AppendToBuffer($"# -> {message}");
	}

	private void AppendToBuffer(string message)
	{
		if(message[message.Length - 1] == '\n')
			message = message.Remove(message.Length - 1);
		message = message.Replace("\n", "\n# + ");

		List<string> text = new List<string>(message.Split('\n'));

		foreach(string line in text) {
			StringBuilder sb = new StringBuilder(line);

			float width = gScreen.MeasureStringInPixels(sb, gScreen.Font, gScreen.FontSize).X;

			if(width >= vTextLimit.X) {
				StringBuilder newline = new StringBuilder();

				List<string> words = new List<string>(line.Split(' '));

				float linewidth = 0.0f;

				foreach(string w in words) {
					sb.Clear();
					sb.Append(w + ' ');

					width = gScreen.MeasureStringInPixels(sb, gScreen.Font, gScreen.FontSize).X;

					if(linewidth + width >= vTextLimit.X) {
						lBuffer.Add(newline.ToString());

						newline.Clear();
						newline.Append("# + " + w + ' ');

						linewidth = gScreen.MeasureStringInPixels(sb, gScreen.Font, gScreen.FontSize).X;
					}
					else {
						newline.Append(w + ' ');

						linewidth += width;
					}
				}
				lBuffer.Add(newline.ToString());
			}
			else lBuffer.Add(line);
		}
	}

	private void WriteBuffer()
	{
		lBuffer.Add("#");

		int height = (int)vTextLimit.Y;

		if(lBuffer.Count < height) {
			string content = gScreen.GetText();

			if(content != string.Empty) {
				if(content.Length > 2)
					content = content.Substring(0, content.Length - 3);

				List<string> previous = new List<string>(content.Split('\n'));

				lBuffer.InsertRange(0, previous);
			}
		}

		if(lBuffer.Count > height)
			lBuffer.RemoveRange(0, lBuffer.Count - height);

		gScreen.WriteText("");

		foreach(string line in lBuffer)
			gScreen.WriteText(line + '\n', true);

		lBuffer.Clear();
	}
}