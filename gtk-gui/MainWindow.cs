
// This file has been generated by the GUI designer. Do not modify.

public partial class MainWindow
{
	private global::Gtk.Fixed fixed2;
	private global::Gtk.Image image1;
	private global::Gtk.Entry txtLogin;
	private global::Gtk.CheckButton chopengl;
	private global::Gtk.Button btnPlay;
	
	protected virtual void Build ()
	{
		global::Stetic.Gui.Initialize (this);
		// Widget MainWindow
		this.Name = "MainWindow";
		this.Title = "MainWindow";
		this.WindowPosition = ((global::Gtk.WindowPosition)(3));
		// Container child MainWindow.Gtk.Container+ContainerChild
		this.fixed2 = new global::Gtk.Fixed ();
		this.fixed2.Name = "fixed2";
		this.fixed2.HasWindow = false;
		// Container child fixed2.Gtk.Fixed+FixedChild
		this.image1 = new global::Gtk.Image ();
		this.image1.Name = "image1";
		this.image1.Pixbuf = global::Gdk.Pixbuf.LoadFromResource ("monotod.336x280.jpg");
		this.fixed2.Add (this.image1);
		global::Gtk.Fixed.FixedChild w1 = ((global::Gtk.Fixed.FixedChild)(this.fixed2 [this.image1]));
		w1.X = 15;
		w1.Y = 13;
		// Container child fixed2.Gtk.Fixed+FixedChild
		this.txtLogin = new global::Gtk.Entry ();
		this.txtLogin.CanFocus = true;
		this.txtLogin.Name = "txtLogin";
		this.txtLogin.Text = "kalurza2";
		this.txtLogin.IsEditable = true;
		this.txtLogin.InvisibleChar = '●';
		this.fixed2.Add (this.txtLogin);
		global::Gtk.Fixed.FixedChild w2 = ((global::Gtk.Fixed.FixedChild)(this.fixed2 [this.txtLogin]));
		w2.X = 15;
		w2.Y = 303;
		// Container child fixed2.Gtk.Fixed+FixedChild
		this.chopengl = new global::Gtk.CheckButton ();
		this.chopengl.CanFocus = true;
		this.chopengl.Name = "chopengl";
		this.chopengl.Label = "-opengl";
		this.chopengl.Active = true;
		this.chopengl.DrawIndicator = true;
		this.chopengl.UseUnderline = true;
		this.fixed2.Add (this.chopengl);
		global::Gtk.Fixed.FixedChild w3 = ((global::Gtk.Fixed.FixedChild)(this.fixed2 [this.chopengl]));
		w3.X = 174;
		w3.Y = 303;
		// Container child fixed2.Gtk.Fixed+FixedChild
		this.btnPlay = new global::Gtk.Button ();
		this.btnPlay.CanFocus = true;
		this.btnPlay.Name = "btnPlay";
		this.btnPlay.UseUnderline = true;
		this.btnPlay.Label = "odpalaj giere !";
		this.fixed2.Add (this.btnPlay);
		global::Gtk.Fixed.FixedChild w4 = ((global::Gtk.Fixed.FixedChild)(this.fixed2 [this.btnPlay]));
		w4.X = 249;
		w4.Y = 299;
		this.Add (this.fixed2);
		if ((this.Child != null)) {
			this.Child.ShowAll ();
		}
		this.DefaultWidth = 365;
		this.DefaultHeight = 337;
		this.Show ();
		this.DeleteEvent += new global::Gtk.DeleteEventHandler (this.OnDeleteEvent);
		this.btnPlay.Clicked += new global::System.EventHandler (this.OnBtnPlayClicked);
	}
}
